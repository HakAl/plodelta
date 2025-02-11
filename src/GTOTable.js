import {Fragment} from "react";
import {GTO_PREFLOP_KEYS, GTO_PREFLOP_VALUES} from "./appData";

const isTenPercentOff = (gto, delta) => {
    return  ((delta / gto) * 100).toFixed() >= 10;
}

function GTOTable({title, playerValues}) {
    return (
        <Fragment>
            <table className="styled-table">
                <thead><tr><th>{title}</th><th>GTO</th><th>YOU</th><th>Δ</th></tr></thead>
                <tbody>
                {GTO_PREFLOP_KEYS.map((title, i) => {
                        return <tr key={i}>
                            <td><b>{title}</b></td>
                            <td>{GTO_PREFLOP_VALUES[i]}</td>
                            {playerValues && <td >{playerValues[i]}</td>
                            }
                            {playerValues &&
                                <td className={isTenPercentOff(GTO_PREFLOP_VALUES[i], (playerValues[i] - GTO_PREFLOP_VALUES[i]).toFixed(1))
                                    ? 'bad' : 'good'}>
                                    {(playerValues[i] - GTO_PREFLOP_VALUES[i]).toFixed(1)}
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
