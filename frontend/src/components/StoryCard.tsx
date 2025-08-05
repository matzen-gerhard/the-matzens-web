import { useState, useRef, useEffect } from "react";
import DOMPurify from "dompurify";
import type { StoryMetadata } from "../api/types";

interface StoryCardProps {
    story: StoryMetadata;
}

export default function StoryCard({ story }: StoryCardProps) {
    const [expanded, setExpanded] = useState(false);
    const [isOverflowing, setIsOverflowing] = useState(false);
    const [showModal, setShowModal] = useState(false);
    const textRef = useRef<HTMLDivElement | null>(null);

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
        <>
            <div className="story-card">
                <div className="image-container">
                    <img
                        src={story.coverImage}
                        alt={story.title}
                        className="story-cover"
                    />
                    {/* Overlay button to open modal */}
                    {story.chapters.length > 0 && (
                        <button
                            className="overlay-btn"
                            onClick={() => setShowModal(true)}
                        >
                            Read
                        </button>
                    )}
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

            {/* Modal for chapter viewer */}
            {showModal && (
                <div className="modal-overlay">
                    <div className="modal-content">
                        <button
                            className="close-btn"
                            onClick={() => setShowModal(false)}
                        >
                            ×
                        </button>
                        <iframe
                            src={story.chapters[0].htmlUri}
                            title={story.chapters[0].title}
                            className="chapter-frame"
                        ></iframe>
                    </div>
                </div>
            )}
        </>
    );
}
