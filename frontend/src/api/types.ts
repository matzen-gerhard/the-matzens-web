export interface FilmMetadata {
    title: string;
    media: string;
    html: string;
}

export interface StoryMetadata {
    title: string;
    chapters: ChapterMetadata[];
}

export interface ChapterMetadata {
    title: string;
    docId: string;
}