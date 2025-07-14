const API_URL = import.meta.env.VITE_API_URL;

export class ApiService {
    private apiUrl: string;

    constructor() {
        this.apiUrl = API_URL ?? ""
    }

    async getVideos() {
        const res = await fetch(`${this.apiUrl}/api/videos`);
        if (!res.ok) {
            throw new Error("Failed to fetch videos");
        }

        return res.json();
    }
}
