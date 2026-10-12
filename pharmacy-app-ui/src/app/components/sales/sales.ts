
import { Component, OnInit, inject,ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HttpErrorResponse } from '@angular/common/http';

import { PharmacyApiService } from '../../services/pharmacy-api.service';
import { Sale } from '../../models/pharmacy.models';

@Component({
  selector: 'app-sales',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './sales.html',
  styleUrl: './sales.css'
})
export class SalesComponent implements OnInit {
  private readonly api = inject(PharmacyApiService);
  private readonly cdr = inject(ChangeDetectorRef);

  sales: Sale[] = [];
  loading = false;
  errorMessage = '';

  ngOnInit(): void {
    this.loadSales();
  }

  
  loadSales(): void {
    this.loading = true;
    this.errorMessage = '';

    this.api.getSales().subscribe({
      next: data => {
        this.sales = data;
        this.loading = false;
        this.cdr.markForCheck();
      },
      error: error => {
        this.loading = false;
        this.errorMessage =
          error.error?.message || 'Unable to load sales history.';
        this.cdr.markForCheck();
      }
    });
  }


  get totalRevenue(): number {
    return this.sales.reduce(
      (sum, sale) => sum + sale.totalAmount, 0
    );
  }

  get totalUnitsSold(): number {
    return this.sales.reduce(
      (sum, sale) => sum + sale.quantitySold, 0
    );
  }
}
