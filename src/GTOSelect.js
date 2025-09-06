// Copyright (c) 2025 HakAl.  See LICENCES/MIT.txt for licence terms.
import {REPORT_VALUES} from "./appData";

function GTOSelect({onReportChange}) {
    return <div className={'input_container'}>
        <label htmlFor="gto-select">Choose GTO Standard:</label>
        <select name="reports" id="reports_select" onChange={(event) => onReportChange(event)}>
            {REPORT_VALUES.map((report) => <option key={report.value} value={report.value}>{report.display}</option>)}
        </select>
    </div>
}

export default GTOSelect;