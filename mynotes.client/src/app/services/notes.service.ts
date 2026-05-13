import { Injectable } from '@angular/core';
import { DataService } from './data.service';
import { ToastrService } from 'ngx-toastr';
import { Observable, tap } from 'rxjs';
import { Note } from '../models/note.model';

@Injectable({
  providedIn: 'root'
})
export class NotesService {
  private getNotesByUserIdEndpoint = `/Notes/GetNotesByUserId`;
  private updateNoteEndpoint = `/Notes/UpdateNote`;

  constructor(
    private dataService: DataService,
    private toastr: ToastrService
  ) {}


  getNotesByUserId(userId: number): Observable<any> {
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

  updateNote(note: Note): Observable<Note> {
    return this.dataService.post<Note>(this.updateNoteEndpoint, note)
      .pipe(
        tap({
          next: (response) => {
            return response;
          },
          error: (error) => {
            this.toastr.error('Failed to update note', 'Error');
            throw error;
          }
        })
      );
  }

  
}
