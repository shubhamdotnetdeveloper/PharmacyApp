
import { Component, OnInit, inject,ChangeDetectorRef, } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  FormBuilder,
  ReactiveFormsModule,
  Validators
} from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { FormsModule } from '@angular/forms';
import { PharmacyApiService } from '../../services/pharmacy-api.service';
import { Medicine } from '../../models/pharmacy.models';

@Component({
  selector: 'app-medicine',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, FormsModule],
  templateUrl: './medicine.html',
  styleUrl: './medicine.css'
})
export class MedicineComponent implements OnInit {
  private readonly api = inject(PharmacyApiService);
  private readonly fb = inject(FormBuilder);
  private readonly cdr = inject(ChangeDetectorRef);

  medicines: Medicine[] = [];
  searchText = '';
  showAddForm = false;
  selectedMedicine: Medicine | null = null;

  loading = false;
  saving = false;
  errorMessage = '';
  successMessage = '';

  medicineForm = this.fb.nonNullable.group({
    fullName: ['', [Validators.required, Validators.maxLength(200)]],
    brand: ['', [Validators.required, Validators.maxLength(100)]],
    notes: [''],
    expiryDate: ['', Validators.required],
    quantity: [0, [Validators.required, Validators.min(0)]],
    price: [0, [Validators.required, Validators.min(0.01)]]
  });

  saleForm = this.fb.nonNullable.group({
    quantitySold: [1, [Validators.required, Validators.min(1)]]
  });

  ngOnInit(): void {
    this.loadMedicines(this.searchText.trim());
  }

  
  loadMedicines(name: string = ''): void {
    this.loading = true;

    this.api.getMedicines(name).subscribe({
      next: data => {
        this.medicines = data;
        this.loading = false;
        this.cdr.markForCheck();
      },
      error: error => {
        this.loading = false;
        this.handleError(error);
        this.cdr.markForCheck();
      }
    });
  }

  searchMedicines(): void {
    this.loadMedicines(this.searchText.trim());
  }


  get filteredMedicines(): Medicine[] {
    const query = this.searchText.trim().toLowerCase();

    return this.medicines.filter(m =>
      m.fullName.toLowerCase().includes(query)
    );
  }

  get totalStock(): number {
    return this.medicines.reduce(
      (sum, m) => sum + m.quantity, 0
    );
  }

  get expiringCount(): number {
    return this.medicines.filter(
      m => this.isExpiring(m)
    ).length;
  }

  get lowStockCount(): number {
    return this.medicines.filter(
      m => m.quantity < 10
    ).length;
  }

  openAddForm(): void {
    this.showAddForm = true;
    this.selectedMedicine = null;
    this.medicineForm.reset();
    this.clearMessages();
  }

  cancelAdd(): void {
    this.showAddForm = false;
  }

  saveMedicine(): void {
    if (this.medicineForm.invalid || this.saving) {
      this.medicineForm.markAllAsTouched();
      return;
    }

    const request = this.medicineForm.getRawValue();

    if (!Number.isInteger(request.quantity) ||
        Math.abs(Math.round(request.price * 100) -
                 request.price * 100) > 1e-8) {
      this.errorMessage =
        'Quantity must be whole and price must have at most 2 decimal places.';
      return;
    }

    this.saving = true;
    this.clearMessages();

    
  this.api.addMedicine(request).subscribe({
    next: () => {
      this.saving = false;
      this.showAddForm = false;
      this.successMessage = 'Medicine added successfully.';

      this.loadMedicines();
      this.cdr.markForCheck();
    },
    error: error => {
      this.saving = false;
      this.handleError(error);
      this.cdr.markForCheck();
    }
  });

  }

  selectMedicine(medicine: Medicine): void {
    this.selectedMedicine = medicine;
    this.showAddForm = false;
    this.saleForm.reset({ quantitySold: 1 });
    this.clearMessages();
  }

  cancelSale(): void {
    this.selectedMedicine = null;
  }

  saveSale(): void {
    if (!this.selectedMedicine ||
        this.saleForm.invalid ||
        this.saving) {
      this.saleForm.markAllAsTouched();
      return;
    }

    const quantity = this.saleForm.getRawValue().quantitySold;

    if (!Number.isInteger(quantity) ||
        quantity > this.selectedMedicine.quantity) {
      this.errorMessage = 'Invalid sale quantity.';
      return;
    }

    this.saving = true;
    this.clearMessages();

    
  this.api.createSale({
    medicineId: this.selectedMedicine.id,
    quantitySold: quantity
  }).subscribe({
    next: () => {
      this.saving = false;
      this.selectedMedicine = null;
      this.successMessage = 'Sale recorded successfully.';

      this.loadMedicines();
      this.cdr.markForCheck();
    },
    error: error => {
      this.saving = false;
      this.handleError(error);
      this.cdr.markForCheck();
    }
  });

  }

  daysToExpiry(medicine: Medicine): number {
    const now = new Date();

    const today = Date.UTC(
      now.getFullYear(),
      now.getMonth(),
      now.getDate()
    );

    const expiry = Date.parse(
      `${medicine.expiryDate}T00:00:00Z`
    );

    return Math.round((expiry - today) / 86400000);
  }

  isExpired(medicine: Medicine): boolean {
    return this.daysToExpiry(medicine) < 0;
  }

  isExpiring(medicine: Medicine): boolean {
    return this.daysToExpiry(medicine) < 30;
  }

  canSell(medicine: Medicine): boolean {
    return !this.isExpired(medicine) &&
           medicine.quantity > 0;
  }

  rowClass(medicine: Medicine): string {
    if (this.isExpiring(medicine)) return 'table-danger';
    if (medicine.quantity < 10) return 'table-warning';
    return '';
  }

  clearMessages(): void {
    this.errorMessage = '';
    this.successMessage = '';
  }

  private handleError(error: HttpErrorResponse): void {
    this.errorMessage =
      error.error?.message ||
      error.error?.title ||
      'Unable to complete the request.';
  }
}
