
import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

import {
  Medicine,
  Sale,
  CreateMedicineRequest,
  CreateSaleRequest
} from '../models/pharmacy.models';

@Injectable({
  providedIn: 'root'
})
export class PharmacyApiService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = '/api';

  getMedicines(name: string = ''): Observable<Medicine[]> {
  const params = new HttpParams().set('name', name);

  return this.http.get<Medicine[]>(
    `${this.baseUrl}/medicines`,
    { params }
  );
}

  addMedicine(
    request: CreateMedicineRequest
  ): Observable<Medicine> {
    return this.http.post<Medicine>(
      `${this.baseUrl}/medicines`,
      request
    );
  }

  getSales(): Observable<Sale[]> {
    return this.http.get<Sale[]>(
      `${this.baseUrl}/sales`
    );
  }

  createSale(
    request: CreateSaleRequest
  ): Observable<Sale> {
    return this.http.post<Sale>(
      `${this.baseUrl}/sales`,
      request
    );
  }
}
