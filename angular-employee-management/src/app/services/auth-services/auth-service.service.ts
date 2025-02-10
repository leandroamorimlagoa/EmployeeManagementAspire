import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { catchError, Observable, of, tap } from 'rxjs';
import { AuthenticatedUserModel } from '../../models/authenticated-user-model';
import { ErrorModalComponent } from '../../components/error-modal/error-modal.component';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private apiUrl = 'http://localhost:5103/api/authenticate';
  private http = inject(HttpClient);
  private router = inject(Router);
  private modalService = inject(NgbModal);

  constructor() { }

  /**
   * Realiza o login e armazena o token e os dados do usuário no localStorage.
   */
  login(email: string, password: string): Observable<{ token: string }> {
    return this.http.post<{ token: string }>(`${this.apiUrl}`, { email, password })
                    .pipe(
                      tap((response) => {
                        localStorage.setItem('token', response.token);
                        this.fetchAuthenticatedUser();
                      }),
                      catchError((error) => {
                        // this.showErrorModal(error.error?.detail || 'Erro ao realizar login.');
                        return of();
                      })
                    );
  }

  /**
   * Obtém os dados do usuário autenticado e armazena no localStorage.
   */
  fetchAuthenticatedUser(): void {
    this.http.get<AuthenticatedUserModel>(`${this.apiUrl}/info`).subscribe({
      next: (user) => {
        localStorage.setItem('user', JSON.stringify(user));
        this.router.navigate(['/main']);
      },
      error: (error) => {
        this.showErrorModal(error.error?.detail || 'Erro ao carregar usuário.');
      },
    });
  }

  /**
   * Retorna `true` se o usuário estiver autenticado.
   */
  isAuthenticated(): boolean {
    if (typeof window === 'undefined') {
      return false;
    }
  
    const token = this.getToken();
    if (!token) {
      this.logout();
      return false;
    }
    return true;
  }

  getToken() : string | null {
    if (typeof window !== 'undefined' && window.localStorage) {
      return localStorage.getItem('token');
    }
    return null;
  }
  /**
   * Obtém os dados do usuário autenticado a partir do localStorage.
   */
  getAuthenticatedUser(): AuthenticatedUserModel | null {
    const user = localStorage.getItem('user');
    return user ? JSON.parse(user) : null;
  }

  /**
   * Faz logout do usuário, limpa os dados do localStorage e redireciona para login.
   */
  logout(): void {
    if( typeof window !== 'undefined' && window.localStorage) {
      localStorage.removeItem('token');
      localStorage.removeItem('user');
      this.router.navigate(['/login']);
    }
  }

  /**
   * Exibe modal de erro.
   */
  private showErrorModal(message: string): void {
    const modalRef = this.modalService.open(ErrorModalComponent, { centered: true });
    modalRef.componentInstance.message = message;
  }
}