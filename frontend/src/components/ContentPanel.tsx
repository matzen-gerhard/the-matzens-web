import { useEffect, useState } from "react";
import { ApiService } from "../api/routes";
import type { FilmMetadata } from "../api/types";

interface Props {
    activeSection: "films" | "stories";
    selectedFilm: FilmMetadata | null;
    selectedStory: string | null;
    onFilmDetailLoaded: (detail: FilmDetail | null) => void; // NEW: Notify parent for iframe
}

export function ContentPanel({ activeSection, selectedFilm, selectedStory, onFilmDetailLoaded }: Props) {
    const [filmDetail, setFilmDetail] = useState<FilmDetail | null>(null);

    useEffect(() => {
        const fetchContent = async () => {
            const apiService = new ApiService();
            if (activeSection === "films" && selectedFilm) {
                try {
                    const data = await apiService.getFilm(selectedFilm.media);
                    setFilmDetail(data);
                    onFilmDetailLoaded(data); // Pass detail to parent
                } catch (err) {
                    console.error("Failed to load film detail", err);
                }

            } else {
                setFilmDetail(null);
                onFilmDetailLoaded(null); // Pass null to parent
            }
        };

        fetchContent();
    }, [activeSection, selectedFilm]);

    if (activeSection === "films") {
        if (!selectedFilm) return <p>Select a film from the sidebar</p>;
        if (!filmDetail) return <p>Loading film details...</p>;

        return (
            <div>
                <video key={filmDetail.mediaUrl} controls style={{ width: "100%", maxWidth: "100%" }}>
                    <source src={filmDetail.mediaUrl} type="video/mp4" />
                    Your browser does not support the video tag.
                </video>
            </div>
        );
    }

    if (activeSection === "stories") {
        return selectedStory ? <div><h2>{selectedStory}</h2><p>Story content coming soon...</p></div>
            : <p>Select a story from the sidebar</p>;
    }

    return <p>Select an item from the sidebar</p>;
}
