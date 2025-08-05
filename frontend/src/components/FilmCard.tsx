import { useState, useEffect, useRef } from "react";
import DOMPurify from "dompurify";
import type { FilmMetadata } from "../api/types";

interface FilmCardProps {
    film: FilmMetadata;
}

export default function FilmCard({ film }: FilmCardProps) {
    const [expanded, setExpanded] = useState(false);
    const [isOverflowing, setIsOverflowing] = useState(false);
    const textRef = useRef<HTMLDivElement | null>(null);

    const sanitizedHtml = DOMPurify.sanitize(film.html, {
        ALLOWED_TAGS: ["p", "strong", "em", "ul", "ol", "li", "br"],
        ALLOWED_ATTR: []
    });

    useEffect(() => {
        const el = textRef.current;
        if (el) {
            setIsOverflowing(el.scrollHeight > el.clientHeight);
        }
    }, [film.html]);

    return (
        <div className="film-card">
            <div className="video-container">
                <video
                    controls
                    src={film.media}
                    style={{ width: "100%", cursor: "pointer" }}
                ></video>
            </div>
            <div className={`description ${expanded ? "expanded" : ""}`}>
                <h3>{film.title}</h3>
                <div
                    ref={textRef}
                    className="description-text"
                    dangerouslySetInnerHTML={{ __html: sanitizedHtml }}
                ></div>
                {isOverflowing && (
                    <button
                        className="expand-btn"
                        onClick={() => setExpanded((prev) => !prev)}
                    >
                        {expanded ? "less" : "more"}
                    </button>
                )}
            </div>
        </div>
    );
}
