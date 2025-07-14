import { useEffect, useState } from "react";
import { ApiService } from "../api/routes";
import type { VideoMetadata } from "../api/types";

export default function VideoList() {
    const [videos, setVideos] = useState<VideoMetadata[]>([]);
    const apiService = new ApiService();

    console.info("Rendering : VideoList.");

    useEffect(() => {
        async function loadVideos() {
            try {
                console.info("Running effect : loadVideos.");
                const vids = await apiService.getVideos();
                setVideos(vids);
                console.info(vids[0].title, vids[0].downloadUri, vids[0].blobName);
            } catch (err) {
                let msg = err instanceof Error && err.message
                    ? err.message
                    : "An unknown error occurred.";
                msg = `Failed to load video effects. ${msg}`;
                console.error(msg);
                alert(`⚠️ ${msg}`);
            }
        }

        loadVideos();
    }, []);

    return (
        <div>
            <h2>🎥 Videos</h2>
            {videos.map(video => (
                <div key={video.blobName} style={{ marginBottom: "1rem" }}>
                    <h4>{video.title}</h4>
                    <video
                        width="480"
                        controls
                        src={video.downloadUri}
                        onError={(e) => {
                            const msg = `Video element failed to load URI: ${video.downloadUri}. Error: ${e.currentTarget?.error}`;
                            console.error(msg);
                            alert(`⚠️ ${msg}`);
                        }}
                    />
                </div>
            ))}
        </div>
    );
}
