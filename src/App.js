import './App.css';
import CSVInput from "./CSVInput";

function App() {
    return (
        <div className="App">
            <header className="App-header">
                <p>Statistics Analyzer</p>
            </header>
            <section>
                <div className={'body-instructions'}>
                    <a href={'https://plomastermind.com/lessons/5-0-analysing-your-stats-in-hem3-2/'}
                       className={'link'}
                       target={'_blank'}>For Context See This Course</a>
                </div>
            </section>
            <section>
                <CSVInput/>
            </section>
        </div>
    );
}

export default App;
