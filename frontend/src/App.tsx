import { useEffect, useState } from "react";
import { ApiService } from "./api/routes";
import "./App.css";
import type { FilmMetadata, StoryMetadata } from "./api/types";
import FilmCard from "./components/FilmCard";

function App() {
  const [films, setFilms] = useState<FilmMetadata[]>([]);
  const [stories, setStories] = useState<StoryMetadata[]>([]);
  const [activeSection, setActiveSection] = useState<"home" | "films" | "stories">("home");

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
          <button onClick={() => setActiveSection("home")}>Home</button>
          <button onClick={() => setActiveSection("films")}>Films</button>
          <button onClick={() => setActiveSection("stories")}>Stories</button>
        </nav>
        <h1 className="site-title">GK Studios</h1>
      </header>

      <main className="main-layout">
        {activeSection === "home" && (
          <div className="home-content">
            <section className="parallax" style={{ backgroundImage: 'url("/gk-with-border.svg")' }}>
              <div className="overlay"></div>
              <h1 className="overlay-text">Welcome to GK Studios</h1>
            </section>
            <div className="parallax-content">
              <h2>Explore our collection of films and stories</h2>
              <p>Discover short films, behind-the-scenes stories, and more.</p>
            </div>
            <section className="parallax" style={{ backgroundImage: 'url("/Epic-Adventures.jpg")' }}>
              <h1>Epic Adventures</h1>
            </section>
            <div className="parallax-content">
              <h2>An epic backpacking trip</h2>
              <p>Earlier this summer, I went on a 28-mile backpacking trip from Mission Peak to Lake Del Valle.</p>
            </div>
          </div>
        )}

        {activeSection === "films" && (
          <div className="films-list">
            {films.map((film) => (
              <FilmCard key={film.media} film={film} />
            ))}
          </div>
        )}

        {activeSection === "stories" && (
          <div className="stories-list">
            {stories.map((story) => (
              <div><h2>{story.title}</h2><p>Story content coming soon...</p></div>
            ))}
          </div>
        )}
      </main>
    </>
  );
}

export default App;
