
export interface Medicine {
  id: string;
  fullName: string;
  notes: string;
  expiryDate: string;
  quantity: number;
  price: number;
  brand: string;
}

export interface CreateMedicineRequest {
  fullName: string;
  notes: string;
  expiryDate: string;
  quantity: number;
  price: number;
  brand: string;
}

export interface Sale {
  id: string;
  medicineId: string;
  medicineName: string;
  quantitySold: number;
  unitPrice: number;
  totalAmount: number;
  soldAt: string;
}

export interface CreateSaleRequest {
  medicineId: string;
  quantitySold: number;
}
