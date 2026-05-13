import { Component, OnInit } from '@angular/core';
import { NotesService } from '../../services/notes.service';
import { AuthService } from '../../services/auth.service';
import { Note } from '../../models/note.model';
import { CdkDragEnd } from '@angular/cdk/drag-drop';

@Component({
  selector: 'app-notes',
  templateUrl: './notes.component.html',
  styleUrl: './notes.component.css'
})
export class NotesComponent implements OnInit {

  userID: number = 0;
  notes: Note[] = [];
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
        this.total = response.total;
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
  
  onDragEnded(event: CdkDragEnd, note: Note) {
    const transform = event.source.getFreeDragPosition();
    note.posX = transform.x;
    note.posY = transform.y;
    this.updateNote(note);
  }

  onResizeEnded(note: Note, width: number, height: number) {
    note.width = width;
    note.height = height;
    this.updateNote(note);
  }

  updateNote(note: Note) {
    this.notesService.updateNote(note).subscribe({
      next: (updatedNote) => {
        // Handle success if needed
      },
      error: (err) => {
        console.error('Failed to update note position/size', err);
      }
    });
  }

  addNote() {
    console.log('Add note clicked');
  }

  unarchiveNote(id: number) {
    const note = this.notes.find(n => n.id === id);
    if (note) {
      note.isArchived = false;
      this.updateNote(note);
    }
  }

  deleteNote(id: number) {
    console.log('Delete note', id);
  }

  archiveNote(id: number) {
    const note = this.notes.find(n => n.id === id);
    if (note) {
      note.isArchived = true;
      this.updateNote(note);
    }
  }

  editNote(id: number) {
    console.log('Edit note', id);
  }

  togglePin(note: Note) {
    note.isPinned = !note.isPinned;
    this.updateNote(note);
  }

  onResizeStart(event: MouseEvent, note: Note, cardElement: HTMLElement) {
    event.preventDefault();
    event.stopPropagation();

    const startX = event.clientX;
    const startY = event.clientY;
    const startWidth = cardElement.offsetWidth;
    const startHeight = cardElement.offsetHeight;

    const onMouseMove = (moveEvent: MouseEvent) => {
      const currentWidth = startWidth + (moveEvent.clientX - startX);
      const currentHeight = startHeight + (moveEvent.clientY - startY);

      note.width = Math.max(150, currentWidth);
      note.height = Math.max(100, currentHeight);
    };

    const onMouseUp = () => {
      document.removeEventListener('mousemove', onMouseMove);
      document.removeEventListener('mouseup', onMouseUp);
      this.updateNote(note);
    };

    document.addEventListener('mousemove', onMouseMove);
    document.addEventListener('mouseup', onMouseUp);
  }
}
