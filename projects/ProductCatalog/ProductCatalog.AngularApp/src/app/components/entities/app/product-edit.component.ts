//@CustomCode
import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { ProductBaseEditComponent } from '@app/components/entities/app/product-base-edit.component';
import { ProductCategory } from '@app-enums/product-category';
import { ProductTone } from '@app-enums/product-tone';
import { DescriptionStatus } from '@app-enums/description-status';
import { N8nProductService } from '@app-services/http/n8n-product.service';

@Component({
  standalone: true,
  selector: 'app-product-edit',
  imports: [CommonModule, FormsModule, TranslateModule],
  templateUrl: './product-edit.component.html',
  styleUrl: './product-edit.component.css'
})
export class ProductEditComponent extends ProductBaseEditComponent {

  public readonly categoryOptions = [
    { value: ProductCategory.Elektronik, labelKey: 'PRODUCT_CATEGORY.ELEKTRONIK' },
    { value: ProductCategory.Mode, labelKey: 'PRODUCT_CATEGORY.MODE' },
    { value: ProductCategory.Haushalt, labelKey: 'PRODUCT_CATEGORY.HAUSHALT' },
    { value: ProductCategory.Sport, labelKey: 'PRODUCT_CATEGORY.SPORT' },
    { value: ProductCategory.Beauty, labelKey: 'PRODUCT_CATEGORY.BEAUTY' },
  ];

  public readonly toneOptions = [
    { value: ProductTone.Luxury, labelKey: 'PRODUCT_TONE.LUXURY' },
    { value: ProductTone.Casual, labelKey: 'PRODUCT_TONE.CASUAL' },
    { value: ProductTone.Technical, labelKey: 'PRODUCT_TONE.TECHNICAL' },
  ];

  public readonly descriptionStatusOptions = [
    { value: DescriptionStatus.None, labelKey: 'DESCRIPTION_STATUS.NONE' },
    { value: DescriptionStatus.Generating, labelKey: 'DESCRIPTION_STATUS.GENERATING' },
    { value: DescriptionStatus.Done, labelKey: 'DESCRIPTION_STATUS.DONE' },
  ];

  public isGenerating = false;

  private n8nProductService = inject(N8nProductService);

  public override get title(): string {
    return this.editMode ? 'PRODUCT_EDIT.EDIT_TITLE' : 'PRODUCT_EDIT.CREATE_TITLE';
  }

  public generateDescriptionWithAI(): void {
    if (!this.dataItem) return;

    this.isGenerating = true;
    this.dataItem.descriptionStatus = DescriptionStatus.Generating;

    this.n8nProductService.generateDescription(this.dataItem).subscribe({
      next: (response) => {
        if (this.dataItem) {
          this.dataItem.description = response.description;
          this.dataItem.tags = response.tags.join(', ');
          this.dataItem.descriptionStatus = DescriptionStatus.Done;
        }
        this.isGenerating = false;
      },
      error: (err) => {
        console.error('Error generating product description via n8n:', err);
        if (this.dataItem) {
          this.dataItem.descriptionStatus = DescriptionStatus.None;
        }
        this.isGenerating = false;
      }
    });
  }
}

