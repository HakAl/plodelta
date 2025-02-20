import './App.css';
import CSVInput from "./CSVInput";
import TheReport from "./Home_Saved Reports_GTOStatAnalyzer.report";

function App() {
    return (
        <div className="App">
            <header className="App-header">
                <h1>GTO Stats Analyzer</h1>
            </header>
            <section>
                <div className={'body-context'}>
                    <h2>The goal of this app is to make it easy to compare your statistics to a GTO baseline using Holdem Manager 3 (HM3).</h2>
                    <p>
                        For additional context <a href={'https://plomastermind.com/lessons/5-0-analysing-your-stats-in-hem3-2/'} className={'link'} target={'_blank'}>see this course</a> and <a href={'https://docs.google.com/spreadsheets/d/1TkjSsPVaCfIKC-JjqfS46LrPDZ3aLJenYWHEjdjeryw/edit?gid=1371458119#gid=1371458119'} className={'link'} target={'_blank'}>this document</a>
                    </p>
                    <h3>How To</h3>
                    <ol>
                        <li>Close HM3.</li>
                        <li>
                            <a href={TheReport}
                               target="_blank"
                               rel="noreferrer"
                               className={'link'}
                               download="Home_Saved Reports_GTOStatAnalyzer.report">Download this HM3 report file.</a></li>
                        <li>Locate the HM3 install directory. EG: "C:\Users\YOUR_NAME\AppData\Roaming\Max Value Software\Holdem Manager\3.0\"</li>
                        <li>Paste the downloaded file into the saved reports directory: EG: C:\Users\YOUR_NAME\AppData\Roaming\Max Value Software\Holdem Manager\3.0\Reports\Saved\</li>
                        <li>Open HM3.</li>
                        <li>In the reports section, select the new GTOStatAnalyzer report.</li>
                        <li>Right click the stats, "Select All".</li>
                        <li>Right click the stats, "Save As".</li>
                        <li>Use the button below to upload the saved file.</li>
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
