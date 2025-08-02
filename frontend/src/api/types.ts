export interface VideoMetadata {
    title: string;
    blobName: string;
    downloadUri: string;
}

export interface FilmMetadata {
    title: string;
    media: string;
    html: string;
}

export interface FilmDetail {
    title: string;
    mediaUrl: string;
    htmlUrl?: string;
}