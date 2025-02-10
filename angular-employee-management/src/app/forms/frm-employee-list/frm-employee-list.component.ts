import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EmployeeModel } from '../../models/employee-model';
import { Router } from '@angular/router';
import { EmployeeServiceService } from '../../services/employee-services/employee-service.service';
import { EmployeeListFilter } from '../../models/employee-list-filter';

@Component({
  selector: 'app-frm-employee-list',
  templateUrl: './frm-employee-list.component.html',
  styleUrls: ['./frm-employee-list.component.css'],
  standalone: true,
  imports: [CommonModule, FormsModule]
})
export class FrmEmployeeListComponent implements OnInit {
  private employeeService = inject(EmployeeServiceService);
  private router = inject(Router);

  employees: EmployeeModel[] = [];
  filter: EmployeeListFilter = new EmployeeListFilter();

  constructor() { }

  ngOnInit() {
    this.fetchEmployees();
  }

  /**
   * Obtém a lista de funcionários do backend com base no filtro.
   */
  fetchEmployees() {
    this.employeeService.getEmployees(this.filter).subscribe({
      next: (response) => {
        console.log('Funcionários:', response);
        this.employees = response;
        // this.filter.totalPages = response.totalPages;
        this.generatePagination();
      },
      error: (error) => {
        console.error('Erro ao buscar funcionários:', error);
      }
    });
  }

  /**
   * Aplica o filtro de pesquisa.
   */
  search() {
    this.filter.currentPage = 1; // Reinicia para a primeira página ao buscar
    this.fetchEmployees();
  }

  /**
   * Remove um funcionário pelo ID.
   */
  deleteEmployee(id: string) {
    if (confirm('Tem certeza que deseja excluir este funcionário?')) {
      this.employeeService.deleteEmployee(id).subscribe({
        next: () => {
          this.fetchEmployees(); // Atualiza a lista após remoção
        },
        error: (error) => {
          console.error('Erro ao excluir funcionário:', error);
        }
      });
    }
  }

  /**
   * Edita um funcionário (navega para a página de edição).
   */
  editEmployee(id: string) {
    console.log('Editando funcionário:', id);
    this.router.navigate(['/employees/edit', id]);
  }

  /**
   * Abre o formulário para adicionar um novo funcionário.
   */
  openEmployeeForm() {
    this.router.navigate(['/employees/edit', '']);
  }

  /**
   * Altera a página da listagem.
   */
  changePage(page: number) {
    if (page >= 1 && page <= this.filter.totalPages) {
      this.filter.currentPage = page;
      this.fetchEmployees();
    }
  }

  /**
   * Gera a numeração da paginação.
   */
  private generatePagination() {
    // this.filter.pages = Array.from({ length: this.filter.totalPages }, (_, i) => i + 1);
  }
}
