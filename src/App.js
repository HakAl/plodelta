import './App.css';
import CSVInput from "./CSVInput";

function App() {
  return (
    <div className="App">
      <header className="App-header">
        <p>PLO (Preflop) Profiler</p>
      </header>
        <section>
            <div className={'body'}>
                <a href={'https://plomastermind.com/courses/plo-profiler/'}
                   className={'link'}
                   target={'_blank'}
                >For Context See This Course</a>
                <ol>
                    <li><p>Create an HM3 report with these stats.</p></li>
                    <li><p>Right click the stats, "Select All".</p></li>
                    <li><p>Right click the stats, "Save As".</p></li>
                    <li><p>Use input below to analyze.</p></li>
                </ol>




            </div>
        </section>
        <section>
            <CSVInput />
        </section>
    </div>
  );
}

export default App;
