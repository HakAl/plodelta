import {Fragment} from "react";
import {GTO_PREFLOP_KEYS} from "./appData";

const isTenPercentOff = (gto, delta) => {
    return  Math.abs(((delta / gto) * 100)).toFixed() >= 10;
}

function GTOTable({title, gtoValues, playerValues}) {
    return (
        <Fragment>
            <table className="styled-table">
                <thead><tr><th>{title}</th><th>GTO</th><th>YOU</th><th>&#916;</th></tr></thead>
                <tbody>
                {GTO_PREFLOP_KEYS.map((title, i) => {
                        return <tr key={i}>
                            <td><b>{title}</b></td>
                            <td>{gtoValues[i]}</td>
                            {playerValues && <td >{playerValues[i]}</td>}
                            {playerValues &&
                                <td className={isTenPercentOff(gtoValues[i], (playerValues[i] - gtoValues[i]).toFixed(1))
                                    ? 'bad' : 'good'}>
                                    {(playerValues[i] - gtoValues[i]).toFixed(1)}
                                </td>
                            }
                        </tr>;
                    }
                )}
                </tbody>
            </table>
        </Fragment>
    );
}

export default GTOTable;
