import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { EmployeeModel } from '../../models/employee-model';
import { EmployeeListFilter } from '../../models/employee-list-filter';

@Injectable({
  providedIn: 'root'
})
export class EmployeeServiceService {
  private apiUrl = 'http://localhost:5103/api/Employee';

  constructor(private http: HttpClient) { }

  private getHeaders(): HttpHeaders {
    const token = localStorage.getItem('token');
    return new HttpHeaders({
      'Content-Type': 'application/json',
      Authorization: `Bearer ${token}`
    });
  }

  getEmployees(filter: EmployeeListFilter): Observable<any> {
    const params = {
      searchTerm: filter.searchTerm,
      page: filter.currentPage.toString(),
      pageSize: filter.pageSize.toString()
    };
    return this.http.get(this.apiUrl, { params });
  }


  getEmployeeById(id: string): Observable<EmployeeModel> {
    return this.http.get<EmployeeModel>(`${this.apiUrl}/${id}`);
  }

  createEmployee(employee: EmployeeModel): Observable<EmployeeModel> {
    return this.http.post<EmployeeModel>(`${this.apiUrl}`, employee);
  }

  updateEmployee(employee: EmployeeModel): Observable<EmployeeModel> {
    return this.http.put<EmployeeModel>(`${this.apiUrl}`, employee);
  }

  deleteEmployee(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

}
