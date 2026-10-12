
import { Component } from '@angular/core';
import { MedicineComponent } from './components/medicine/medicine';
import { SalesComponent } from './components/sales/sales';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [MedicineComponent, SalesComponent],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class App {
  activePage: 'medicines' | 'sales' = 'medicines';

  navigate(page: 'medicines' | 'sales'): void {
    this.activePage = page;
  }
}
