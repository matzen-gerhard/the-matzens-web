import { useEffect, useState } from "react";
import { ApiService } from "./api/routes";
import "./App.css";
import { ContentPanel } from "./components/ContentPanel";
import type { FilmMetadata } from "./api/types";
import type { FilmDetail } from "./api/types";

function App() {
  const [films, setFilms] = useState<FilmMetadata[]>([]);
  const [stories, setStories] = useState<string[]>([]);
  const [activeSection, setActiveSection] = useState<"films" | "stories">("films");
  const [selectedFilm, setSelectedFilm] = useState<FilmMetadata | null>(null);
  const [selectedStory, setSelectedStory] = useState<string | null>(null);
  // Store film detail so we can use it in the sidebar (for iframe)
  const [filmDetail, setFilmDetail] = useState<FilmDetail | null>(null);

  // Fetch content lists on mount
  useEffect(() => {
    const fetchContent = async () => {
      try {
        const apiService = new ApiService();
        const data = await apiService.getContent();
        setFilms(data.films || []);
        setStories(data.stories || []);

        if (data.films?.length) {
          setSelectedFilm(data.films[0]);
        }
        if (data.stories?.length) {
          setSelectedStory(data.stories[0]);
        }

      } catch (err) {
        console.error("Failed to load content", err);
      }
    };

    fetchContent();
  }, []);

  return (
    <>

      <header className="shared-panel top-bar">
        <nav className="nav-links">
          <button onClick={() => setActiveSection("films")}>Films</button>
          <button onClick={() => setActiveSection("stories")}>Stories</button>
        </nav>
        <h1 className="site-title">GK Studios</h1>
      </header>

      <main className="main-layout">
        <section className="content">
          <ContentPanel
            activeSection={activeSection}
            selectedFilm={selectedFilm}
            selectedStory={selectedStory}
            onFilmDetailLoaded={(detail) => setFilmDetail(detail)}
          />
        </section>

        <aside className="sidebar">
          <div className="shared-panel">
            <h2>{activeSection === "films" ? "Films" : "Stories"}</h2>
            <div className="sidebar-items">
              {activeSection === "films"
                ? films.map((film) => (
                  <button onClick={() => setSelectedFilm(film)}>{film.title}</button>
                ))
                : stories.map((story) => (
                  <button onClick={() => setSelectedStory(story)}>{story}</button>
                ))}
            </div>
          </div>

          {activeSection === "films" && filmDetail?.htmlUrl && (
            <div className="shared-panel" style={{ marginTop: "12px" }}>
              <h2>{filmDetail.title}</h2>
              <iframe
                src={filmDetail.htmlUrl}
                width="100%"
                height="300"
                style={{ border: "none" }}
                title="Film Info"
              ></iframe>
            </div>
          )}

        </aside>

      </main>
    </>
  );
}

export default App;
