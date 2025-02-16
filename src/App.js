import './App.css';
import CSVInput from "./CSVInput";

function App() {
    return (
        <div className="App">
            <header className="App-header">
                <p>PLO Preflop Stats Analyzer</p>
            </header>
            <section>
                <div className={'body-instructions'}>
                    <a href={'https://plomastermind.com/lessons/5-0-analysing-your-stats-in-hem3-2/'}
                       className={'link'}
                       target={'_blank'}>For Context See This Course</a>
                    <ol>
                        <li><p>Create an HM3 report with these stats.</p></li>
                        <li><p>Right click the stats, "Select All".</p></li>
                        <li><p>Right click the stats, "Save As".</p></li>
                        <li><p>Use input below to analyze.</p></li>
                    </ol>
                </div>
            </section>
            <section>
                <CSVInput/>
            </section>
        </div>
    );
}

export default App;
