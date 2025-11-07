import { Injectable } from '@angular/core';
import { DataService } from './data.service';
import { ToastrService } from 'ngx-toastr';
import { Observable, tap } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class NotesService {
  private getNotesByUserIdEndpoint = `/Notes/GetNotesByUserId`;

  constructor(
    private dataService: DataService,
    private toastr: ToastrService
  ) {}


  getNotesByUserId(userId): Observable<any> {
    const model = {
      Id: userId
    };
    return this.dataService.post<any>(this.getNotesByUserIdEndpoint, model)
      .pipe(
        tap({
          next: (response) => {
            return response;
          },
          error: (error) => {
            this.toastr.error('Failed to get notes', 'Error');
            throw error;
          }
        })
      );
  }

  
}
