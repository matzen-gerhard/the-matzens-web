import { useEffect, useState } from "react";
import { ApiService } from "./api/routes";
import "./App.css";
import { ContentPanel } from "./components/ContentPanel";
import type { FilmMetadata } from "./api/types";

function App() {
  const [films, setFilms] = useState<FilmMetadata[]>([]);
  const [stories, setStories] = useState<string[]>([]);
  const [activeSection, setActiveSection] = useState<"films" | "stories">("films");
  const [selectedFilm, setSelectedFilm] = useState<FilmMetadata | null>(null);
  const [selectedStory, setSelectedStory] = useState<string | null>(null);
  // Store film detail so we can use it in the sidebar (for iframe)
  const [filmDetail, setFilmDetail] = useState<{ htmlUrl?: string } | null>(null);

  // Fetch content lists on mount
  useEffect(() => {
    const fetchContent = async () => {
      try {
        const apiService = new ApiService();
        const data = await apiService.getContent();
        setFilms(data.films || []);
        setStories(data.stories || []);
      } catch (err) {
        console.error("Failed to load content", err);
      }
    };

    fetchContent();
  }, []);

  return (
    <>

      <header className="top-bar">
        <nav className="nav-links">
          <button onClick={() => setActiveSection("films")}>Films</button>
          <button onClick={() => setActiveSection("stories")}>Stories</button>
        </nav>
        <h1 className="site-title">Griffin's Portfolio</h1>
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
          <h3>{activeSection === "films" ? "Films" : "Stories"}</h3>
          <ul>
            {activeSection === "films"
              ? films.map((film) => (
                <li key={film.media}>
                  <button onClick={() => setSelectedFilm(film)}>{film.title}</button>
                </li>
              ))
              : stories.map((story) => (
                <li key={story}>
                  <button onClick={() => setSelectedStory(story)}>{story}</button>
                </li>
              ))}
          </ul>

          {activeSection === "films" && filmDetail?.htmlUrl && (
            <iframe
              src={filmDetail.htmlUrl}
              width="100%"
              height="300"
              style={{ border: "none", marginTop: "12px" }}
              title="Film Info"
            ></iframe>
          )}
        </aside>
      </main>
    </>
  );
}

export default App;
