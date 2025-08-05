import { useState, useRef, useEffect } from "react";
import DOMPurify from "dompurify";
import type { StoryMetadata } from "../api/types";

interface StoryCardProps {
    story: StoryMetadata;
}

export default function StoryCard({ story }: StoryCardProps) {
    const [expanded, setExpanded] = useState(false);
    const [isOverflowing, setIsOverflowing] = useState(false);
    const textRef = useRef<HTMLDivElement | null>(null);

    // Sanitize HTML excerpt for safe rendering
    const sanitizedHtml = DOMPurify.sanitize(story.html, {
        ALLOWED_TAGS: ["p", "strong", "em", "ul", "ol", "li", "br"],
        ALLOWED_ATTR: []
    });

    useEffect(() => {
        if (textRef.current) {
            setIsOverflowing(textRef.current.scrollHeight > textRef.current.clientHeight);
        }
    }, [story.html]);

    return (
        <div className="story-card">
            {/* Cover image with optional click to open details */}
            <div className="image-container">
                <img
                    src={story.coverImage}
                    alt={story.title}
                    className="story-cover"
                />
            </div>

            <div className={`description ${expanded ? "expanded" : ""}`}>
                <h3 className="description-header">{story.title}</h3>
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
