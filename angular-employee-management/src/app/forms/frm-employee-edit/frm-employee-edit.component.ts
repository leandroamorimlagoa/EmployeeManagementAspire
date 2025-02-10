import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EmployeeModel } from '../../models/employee-model';
import { RegisterPhoneModel } from '../../models/register-phone-model';
import { ActivatedRoute, Router } from '@angular/router';
import { EmployeeServiceService } from '../../services/employee-services/employee-service.service';
import { EmployeeListFilter } from '../../models/employee-list-filter';

@Component({
  selector: 'app-frm-employee-edit',
  templateUrl: './frm-employee-edit.component.html',
  styleUrls: ['./frm-employee-edit.component.css'],
  imports: [CommonModule, FormsModule]
})
export class FrmEmployeeEditComponent implements OnInit {
  employee: EmployeeModel = new EmployeeModel();
  isEditMode: boolean = false;
  docTypes: any[] = [];
  roles: any[] = [];
  managerList: EmployeeModel[] = [];
  filter: EmployeeListFilter = new EmployeeListFilter();

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private employeeService: EmployeeServiceService
  ) { }

  ngOnInit() {
    this.loadDocTypes();
    this.loadRoles();
    this.loadManager(null);

    this.loadEmployee();
  }

  loadManager(currentUserId: string | null) {
    this.filter.pageSize = 100;
    this.employeeService.getEmployees(this.filter).subscribe((data: EmployeeModel[]) => {
      this.managerList = currentUserId ? data.filter(e => e.id !== currentUserId) : data;
    });
  }

  loadEmployee() {
    this.route.paramMap.subscribe(params => {
      const id = params.get('id');
      if (id === '0' || id === null || id === undefined || !id) {
        // Novo funcionário → Não busca dados do backend
        this.isEditMode = false;
        this.employee = new EmployeeModel();
      } else {
        // Editar funcionário → Buscar no backend
        this.isEditMode = true;
        this.requestEmployee(id!);
      }
    });
  }

  requestEmployee(id: string) {
    this.employeeService.getEmployeeById(id).subscribe(data => {
      this.employee = data;
      if (this.employee.birthDate) {
        this.employee.birthDate = this.formatDateForInput(this.employee.birthDate);
      }
      this.loadManager(this.employee.id);
    });
  }

  loadRoles() {
    this.roles = [
      { id: 1, name: 'Employee' },
      { id: 2, name: 'Leader' },
      { id: 3, name: 'Director' }
    ];
  }

  loadDocTypes() {
    this.docTypes = [
      { id: 1, name: 'CPF' },
      { id: 2, name: 'Passport' },
      { id: 3, name: 'Other' }
    ];
  }

  private formatDateForInput(dateString: string): string {
    // Converte a string da API para um objeto Date
    const date = new Date(dateString);

    // Retorna a data formatada como YYYY-MM-DD para o input[type="date"]
    return date.toISOString().split('T')[0];
  }

  saveEmployee() {
    console.log('Salvando funcionário====>', this.employee);
    if (this.isEditMode) {
      this.employeeService.updateEmployee(this.employee).subscribe(() => {
        this.router.navigate(['/employees']);
      });
    } else {
      this.employeeService.createEmployee(this.employee).subscribe(() => {
        this.router.navigate(['/employees']);
      });
    }
  }

  close() {
    console.log('Fechar');
  }

  addPhone() {
    if(!this.employee.phones) {
      this.employee.phones = [];
    }
    this.employee.phones.push(new RegisterPhoneModel());
  }

  removePhone(index: number) {
    this.employee.phones.splice(index, 1);
  }
}
