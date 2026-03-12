//@CustomCode
import { IdType, IdDefault } from '@app/models/i-key-model';
import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { TranslateModule } from '@ngx-translate/core';
import { IQueryParams } from '@app/models/base/i-query-params';
import { IProduct } from '@app-models/entities/app/i-product';
import { ProductBaseListComponent }from '@app/components/entities/app/product-base-list.component';
import { ProductEditComponent }from '@app/components/entities/app/product-edit.component';
@Component({
  standalone: true,
  selector:'app-product-list',
  imports: [ CommonModule, FormsModule, TranslateModule, RouterModule ],
  templateUrl: './product-list.component.html',
  styleUrl: './product-list.component.css'
})
export class ProductListComponent extends ProductBaseListComponent {
  constructor()
  {
    super();
  }
  override ngOnInit(): void {
    super.ngOnInit();
    this.reloadData();
  }
  override prepareQueryParams(queryParams: IQueryParams): void {
    super.prepareQueryParams(queryParams);
    queryParams.filter = 'name.ToLower().Contains(@0) OR description.ToLower().Contains(@0) OR tags.ToLower().Contains(@0)';
  }
  protected override getItemKey(item: IProduct): IdType {
    return item?.id || IdDefault;
  }
  override get pageTitle(): string {
    return 'Products';
  }
  override getEditComponent() {
    return ProductEditComponent;
  }
}
