export interface Note {
    id: number;
    title: string;
    content: string;
    createdAt: Date;
    modifiedAt: Date;
    authorId: number;
    authorName?: string;
    isPinned: boolean;
    colorTag: string;
    color: string;
    tags: string;
    isArchived: boolean;
    posX: number;
    posY: number;
    width: number;
    height: number;
    sortOrder: number;
    zIndex: number;
    isDeleted?: boolean;
}
