export interface FilmMetadata {
    title: string;
    media: string;
    html: string;
}

export interface StoryMetadata {
    title: string;
    coverImage: string;
    html: string;
    chapters: ChapterMetadata[];
}

export interface ChapterMetadata {
    title: string;
    htmlUri: string;
}
