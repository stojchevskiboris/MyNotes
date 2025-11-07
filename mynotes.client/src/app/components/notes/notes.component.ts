import { Component } from '@angular/core';
import { NotesService } from '../../services/notes.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-notes',
  templateUrl: './notes.component.html',
  styleUrl: './notes.component.css'
})
export class NotesComponent {

  userID: number = 0;
  notes: any[] = [];
  total: number = 0;

  constructor(
    private authService: AuthService,
    private notesService: NotesService
  ) {
    this.userID = this.authService.getUserId();
  }

  ngOnInit() {
    this.loadNotes();
  }

  loadNotes() {
    this.notesService.getNotesByUserId(this.userID).subscribe({
      next: (response) => {
        this.notes = response.notes;
        this.total = this.total;
      },
      error: (error) => {
        console.error('Error loading notes:', error);
      }
    });
  }

  get pinnedNotes() {
    return this.notes?.filter(n => n.isPinned && !n.isArchived) ?? [];
  }

  get otherNotes() {
    return this.notes?.filter(n => !n.isPinned && !n.isArchived) ?? [];
  }

  get archivedNotes() {
    return this.notes?.filter(n => n.isArchived) ?? [];
  }
  
  addNote() {
    throw new Error('Method not implemented.');
  }
  unarchiveNote(arg0: any) {
    throw new Error('Method not implemented.');
  }
  deleteNote(arg0: any) {
    throw new Error('Method not implemented.');
  }
  archiveNote(arg0: any) {
    throw new Error('Method not implemented.');
  }
  editNote(arg0: any) {
    throw new Error('Method not implemented.');
  }


}