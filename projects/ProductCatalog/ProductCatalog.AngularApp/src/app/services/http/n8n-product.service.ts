import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '@environment/environment';
import { IProduct } from '@app-models/entities/app/i-product';
import { ProductCategory } from '@app-enums/product-category';
import { ProductTone } from '@app-enums/product-tone';

export interface IN8nProductDescriptionResponse {
  description: string;
  tags: string[];
  productId: string;
}

@Injectable({ providedIn: 'root' })
export class N8nProductService {

  constructor(private http: HttpClient) {}

  generateDescription(product: IProduct): Observable<IN8nProductDescriptionResponse> {
    const categoryName = ProductCategory[product.category] ?? String(product.category);
    const toneName = product.tone != null
      ? (ProductTone[product.tone]?.toLowerCase() ?? 'casual')
      : 'casual';

    const payload = {
      product: {
        id: String(product.id),
        name: product.name,
        category: categoryName,
        price: product.price,
        stock: product.stock,
        tone: toneName,
      },
      timestamp: new Date().toISOString(),
    };

    return this.http.post<IN8nProductDescriptionResponse>(
      environment.N8N_PRODUCT_DESCRIPTION_URL,
      payload
    );
  }
}
