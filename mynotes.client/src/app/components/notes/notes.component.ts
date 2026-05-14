import { Component, OnInit } from '@angular/core';
import { NotesService } from '../../services/notes.service';
import { AuthService } from '../../services/auth.service';
import { Note } from '../../models/note.model';
import { CdkDragEnd } from '@angular/cdk/drag-drop';
import { HostListener, ElementRef } from '@angular/core';

@Component({
  selector: 'app-notes',
  templateUrl: './notes.component.html',
  styleUrl: './notes.component.css'
})
export class NotesComponent implements OnInit {

  userID: number = 0;
  notes: Note[] = [];
  total: number = 0;
  searchText: string = '';
  editingNoteId: number | null = null;

  constructor(private eRef: ElementRef,
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

  get filteredNotes() {
    if (!this.searchText) return this.notes;
    const search = this.searchText.toLowerCase();
    return this.notes.filter(n =>
      (n.title?.toLowerCase() ?? '').includes(search) ||
      (n.content?.toLowerCase() ?? '').includes(search) ||
      (n.tags?.toLowerCase() ?? '').includes(search)
    );
  }

  get pinnedNotes() {
    return this.filteredNotes?.filter(n => n.isPinned && !n.isArchived) ?? [];
  }

  get otherNotes() {
    return this.filteredNotes?.filter(n => !n.isPinned && !n.isArchived) ?? [];
  }

  get archivedNotes() {
    return this.filteredNotes?.filter(n => n.isArchived) ?? [];
  }
  
  onDragEnded(event: CdkDragEnd, note: Note) {
    const transform = event.source.getFreeDragPosition();
    note.posX = Math.max(0, transform.x);
    note.posY = Math.max(0, transform.y);
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
    let posX = 50;
    let posY = 50;
    const activeNotes = this.notes.filter(n => !n.isArchived);
    let isOccupied = true;
    while (isOccupied) {
      isOccupied = activeNotes.some(n => n.posX === posX && n.posY === posY);
      if (isOccupied) {
        posX += 10;
        posY += 10;
      }
    }
    const newNote: Note = {
      id: 0,
      title: 'New Note',
      content: '',
      createdAt: new Date(),
      modifiedAt: new Date(),
      authorId: this.userID,
      isPinned: false,
      isArchived: false,
      colorTag: '#ffffff',
      tags: '',
      posX: posX,
      posY: posY,
      width: 250,
      height: 200,
      sortOrder: 0
    };
    this.notesService.createNote(newNote).subscribe({
      next: (note) => {
        const maxSortOrder = this.notes.length > 0 ? Math.max(...this.notes.map(n => n.sortOrder ?? 0)) : 0;
        note.sortOrder = maxSortOrder + 1;
        this.updateNote(note);
        this.notes.unshift(note);
      },
      error: (err) => {
        console.error('Failed to create note', err);
      }
    });
  }

  unarchiveNote(id: number) {
    const note = this.notes.find(n => n.id === id);
    if (note) {
      note.isArchived = false;
      this.updateNote(note);
    }
  }

  deleteNote(id: number) {
    if (confirm('Are you sure you want to delete this note?')) {
      this.notesService.deleteNote(id).subscribe({
        next: () => {
          this.notes = this.notes.filter(n => n.id !== id);
        },
        error: (err) => {
          console.error('Failed to delete note', err);
        }
      });
    }
  }

  archiveNote(id: number) {
    const note = this.notes.find(n => n.id === id);
    if (note) {
      note.isArchived = true;
      this.updateNote(note);
    }
  }

  editNote(id: number) {
    this.editingNoteId = id;
  }

  saveNote(note: Note) {
    this.notesService.updateNote(note).subscribe({
      next: (updatedNote) => {
        this.editingNoteId = null;
      },
      error: (err) => {
        console.error('Failed to update note', err);
      }
    });
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

  @HostListener('document:click', ['$event'])
  clickout(event: MouseEvent) {
    if (this.editingNoteId !== null) {
      const noteElements = document.querySelectorAll('.note-wrapper');
      let insideNote = false;
      noteElements.forEach(el => {
        if (el.contains(event.target as Node)) {
          insideNote = true;
        }
      });

      if (!insideNote) {
        const noteToSave = this.notes.find(n => n.id === this.editingNoteId);
        if (noteToSave) {
          this.saveNote(noteToSave);
        }
      }
    }
  }

  moveUp(note: Note) {
    const activeNotes = this.notes.filter(n => !n.isArchived).sort((a, b) => (b.sortOrder ?? 0) - (a.sortOrder ?? 0));
    const index = activeNotes.findIndex(n => n.id === note.id);
    if (index > 0) {
      const prevNote = activeNotes[index - 1];
      const tempOrder = note.sortOrder;
      note.sortOrder = prevNote.sortOrder;
      prevNote.sortOrder = tempOrder;
      this.updateNote(note);
      this.updateNote(prevNote);
      this.notes.sort((a, b) => (b.sortOrder ?? 0) - (a.sortOrder ?? 0));
    }
  }

  moveDown(note: Note) {
    const activeNotes = this.notes.filter(n => !n.isArchived).sort((a, b) => (b.sortOrder ?? 0) - (a.sortOrder ?? 0));
    const index = activeNotes.findIndex(n => n.id === note.id);
    if (index < activeNotes.length - 1) {
      const nextNote = activeNotes[index + 1];
      const tempOrder = note.sortOrder;
      note.sortOrder = nextNote.sortOrder;
      nextNote.sortOrder = tempOrder;
      this.updateNote(note);
      this.updateNote(nextNote);
      this.notes.sort((a, b) => (b.sortOrder ?? 0) - (a.sortOrder ?? 0));
    }
  }

}