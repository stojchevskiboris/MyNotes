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
  user: any = null;
  notes: Note[] = [];
  total: number = 0;
  searchText: string = '';
  editingNoteId: number | null = null;
  maxZIndex: number = 0;

  // Modals state
  showDeleteModal: boolean = false;
  noteToDeleteId: number | null = null;
  showSettingsModal: boolean = false;
  settingsUser: any = null;

  constructor(private eRef: ElementRef,
    private authService: AuthService,
    private notesService: NotesService
  ) {
    this.userID = this.authService.getUserId();
    this.user = this.authService.getUser();
  }

  ngOnInit() {
    this.loadNotes();
  }

  loadNotes() {
    this.notesService.getNotesByUserId(this.userID).subscribe({
      next: (response) => {
        this.notes = response.notes;
        this.total = response.total;
        if (this.notes.length > 0) {
          this.maxZIndex = Math.max(...this.notes.map(n => n.zIndex ?? 0));
        }
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
    event.source.reset();
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

    this.maxZIndex++;

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
      color: '#ffffff',
      tags: '',
      posX: posX,
      posY: posY,
      width: 250,
      height: 200,
      sortOrder: 0,
      zIndex: this.maxZIndex
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

  confirmDeleteNote(id: number) {
    this.noteToDeleteId = id;
    this.showDeleteModal = true;
  }

  deleteNote() {
    if (this.noteToDeleteId) {
      this.notesService.deleteNote(this.noteToDeleteId).subscribe({
        next: () => {
          this.notes = this.notes.filter(n => n.id !== this.noteToDeleteId);
          this.showDeleteModal = false;
          this.noteToDeleteId = null;
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
    const note = this.notes.find(n => n.id === id);
    if (note) {
      this.bringToFront(note);
    }
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

  onNoteMouseDown(note: Note) {
    this.bringToFront(note);
  }

  bringToFront(note: Note) {
    if (note.zIndex < this.maxZIndex) {
      this.maxZIndex++;
      note.zIndex = this.maxZIndex;
      this.updateNote(note);
    } else if (this.maxZIndex === 0) {
      this.maxZIndex = 1;
      note.zIndex = 1;
      this.updateNote(note);
    }
  }

  onResizeStart(event: MouseEvent, note: Note, cardElement: HTMLElement) {
    event.preventDefault();
    event.stopPropagation();
    this.bringToFront(note);

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

  logout() {
    this.authService.logout();
  }

  openSettings() {
    this.settingsUser = { ...this.user };
    this.showSettingsModal = true;
  }

  saveSettings() {
    this.authService.updateUser(this.settingsUser).subscribe({
      next: (updatedUser) => {
        this.user = updatedUser;
        this.showSettingsModal = false;
      }
    });
  }

}
