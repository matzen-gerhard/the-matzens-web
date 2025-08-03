const API_URL = import.meta.env.VITE_API_URL;

export class ApiService {
    private apiUrl: string;

    constructor() {
        this.apiUrl = API_URL ?? ""
    }

    async getContent() {
        const res = await fetch(`${this.apiUrl}/api/content`);
        if (!res.ok) {
            throw new Error("Failed to fetch content");
        }
        return res.json();
    }

    async getFilm(media: string) {
        const res = await fetch(`${this.apiUrl}/api/film/${encodeURIComponent(media)}`);
        if (!res.ok) {
            throw new Error("Failed to fetch film metadata");
        }
        return res.json();
    }
}

/*
    IN DEVELOPMENT:
    .env file (frontend)  <---MUST MATCH---> local.settings.json (Azure Functions backend)
    
    IN PROEUCTION:
    API_URL will be and empty string.
    
    /api/* will be proxied to the Azure funciton backend. 

    Example of Azure Function:
    [Function("videos")]
    public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)

    Then the route will be accessable via:
    https://<your-static-web-app>.azurestaticapps.net/api/videos
*/
