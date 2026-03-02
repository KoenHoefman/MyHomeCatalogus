using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MyHomeCatalogus.Components.Extensions;
using MyHomeCatalogus.Components.Toast;
using MyHomeCatalogus.Interfaces;
using MyHomeCatalogus.Shared.Domain;
using MyHomeCatalogus.Shared.Exceptions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace MyHomeCatalogus.Components.Pages.ProductPages
{
	public partial class ProductEdit
	{
		[Inject]
		public required IProductService ProductService { get; set; }

		[Inject]
		public required IProductTypeService ProductTypeService { get; set; }

		[Inject]
		public required IStoreService StoreService { get; set; }

		[Inject]
		public required IPurchaseUnitService PurchaseUnitService { get; set; }

		[Inject]
		public required NavigationManager NavigationManager { get; set; }

		[Inject]
		public required IToastService ToastService { get; set; }

		[Parameter]
		public int Id { get; set; }

		[SupplyParameterFromForm]
		private Product? Product { get; set; }

		private string? _message;
		private bool _isProcessing;

		private EditContext EditContext { get; set; } = null!;

		private IEnumerable<ProductType> _productTypes = new List<ProductType>();
		private IEnumerable<Store> _stores = new List<Store>();
		private IEnumerable<PurchaseUnit> _purchaseUnits = new List<PurchaseUnit>();

		protected override async Task OnInitializedAsync()
		{
			try
			{
				Product ??= await ProductService.Get(Id);

				EditContext = new EditContext(Product);

				var productTypesTask = ProductTypeService.GetAll();
				var storesTask = StoreService.GetAll();
				var purchaseUnitsTask = PurchaseUnitService.GetAll();

				await Task.WhenAll(productTypesTask, storesTask, purchaseUnitsTask);

				_productTypes = productTypesTask.Result;
				_stores = storesTask.Result;
				_purchaseUnits = purchaseUnitsTask.Result;

				if (Product.Picture is not null)
				{
					_picturePreviewUrl =
						$"data:{Product.PictureMimeType};base64,{Convert.ToBase64String(Product.Picture)}";
				}
				if (Product.Barcode is not null)
				{
					_barcodePreviewUrl =
						$"data:{Product.BarcodeMimeType};base64,{Convert.ToBase64String(Product.Barcode)}";
				}
			}
			catch (KeyNotFoundException)
			{
				_message = "Product not found.";
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
				_message = $"Error fetching data: {ex.Message}";
			}
		}

		private async Task UpdateProduct()
		{
			if (_isProcessing)
			{
				return;
			}

			try
			{
				_isProcessing = true;
				_message = null;

				var updatedEntity = await ProductService.Update(Product!);

				ToastService.ShowToast($"Product '{updatedEntity.Name}' was successfully updated.", ToastLevel.Success);

				NavigationManager.NavigateTo(RouteConstants.GetDetailRoute(RouteConstants.ProductBaseRoute, updatedEntity.Id));
			}
			catch (UniqueConstraintException uex)
			{
				_isProcessing = false;

				EditContext.AddValidationErrors(uex);
			}
			catch (Exception ex)
			{
				_isProcessing = false;

				Console.WriteLine(ex);

				ToastService.ShowToast($"Error updating product: {ex.Message}", ToastLevel.Error, true);
			}
		}

		#region Picture and barcode

		private const long MaxInputFileSize = 1024 * 1024 * 5; // 5MB max for reading stream
		private const int TargetMaxPictureSizeKb = 500;        // Target file size *after* shrinking (in KB)

		private const int PictureTargetWidth = 600;
		private const int PictureTargetHeight = 800;
		private string? _picturePreviewUrl;

		private const int BarcodeTargetWidth = 800;
		private const int BarcodeTargetHeight = 600;
		private string? _barcodePreviewUrl;

		private async Task LoadPicture(InputFileChangeEventArgs e)
		{
			var file = e.File;

			if (file.Size > MaxInputFileSize)
			{
				var messages = new ValidationMessageStore(EditContext);
				messages.Add(() => Product!.Picture!, $"Picture file is too large (max {MaxInputFileSize / 1024 / 1024}MB).");
				EditContext.NotifyValidationStateChanged();
				return;
			}

			await using var stream = file.OpenReadStream();

			try
			{
				// Apply resizing, constrained to the target dimensions
				using var image = await Image.LoadAsync(stream);
				image.Mutate(x => x.Resize(new ResizeOptions
				{
					Size = new Size(PictureTargetWidth, PictureTargetHeight),
					Mode = ResizeMode.Max // Keeps aspect ratio, fits within bounds
				}));

				using var outputStream = new MemoryStream();
				await image.SaveAsJpegAsync(outputStream);

				// If the resulting JPEG is still too large, you might need to adjust quality/size.
				if (outputStream.Length > TargetMaxPictureSizeKb * 1024)
				{
					var messages = new ValidationMessageStore(EditContext);
					messages.Add(() => Product!.Picture!, $"Could not shrink image sufficiently. Please upload a smaller original file.");
					EditContext.NotifyValidationStateChanged();

					return;
				}

				Product!.Picture = outputStream.ToArray();
				Product.PictureMimeType = "image/jpeg";

				_picturePreviewUrl = $"data:image/jpeg;base64,{Convert.ToBase64String(Product.Picture)}";
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Image processing error: {ex.Message}");
				var messages = new ValidationMessageStore(EditContext);
				messages.Add(() => Product!.Picture!, "An error occurred during image processing.");
				EditContext.NotifyValidationStateChanged();
			}
		}
		private async Task LoadBarcode(InputFileChangeEventArgs e)
		{
			var file = e.File;

			if (file.Size > MaxInputFileSize)
			{
				var messages = new ValidationMessageStore(EditContext);
				messages.Add(() => Product!.Barcode!, $"Barcode file is too large (max {MaxInputFileSize / 1024 / 1024}MB).");
				EditContext.NotifyValidationStateChanged();
				return;
			}

			await using var stream = file.OpenReadStream();

			try
			{
				// Apply resizing, constrained to the target dimensions
				using var image = await Image.LoadAsync(stream);
				image.Mutate(x => x.Resize(new ResizeOptions
				{
					Size = new Size(BarcodeTargetWidth, BarcodeTargetHeight),
					Mode = ResizeMode.Max // Keeps aspect ratio, fits within bounds
				}));

				using var outputStream = new MemoryStream();
				await image.SaveAsJpegAsync(outputStream);

				// If the resulting JPEG is still too large, you might need to adjust quality/size.
				if (outputStream.Length > TargetMaxPictureSizeKb * 1024)
				{
					var messages = new ValidationMessageStore(EditContext);
					messages.Add(() => Product!.Barcode!, $"Could not shrink image sufficiently. Please upload a smaller original file.");
					EditContext.NotifyValidationStateChanged();

					return;
				}

				Product!.Barcode = outputStream.ToArray();
				Product.BarcodeMimeType = "image/jpeg";

				_barcodePreviewUrl = $"data:image/jpeg;base64,{Convert.ToBase64String(Product.Barcode)}";
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Image processing error: {ex.Message}");
				var messages = new ValidationMessageStore(EditContext);
				messages.Add(() => Product!.Barcode!, "An error occurred during image processing.");
			}
		}

		#endregion

	}
}
