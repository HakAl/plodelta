import {Fragment} from "react";

const isTenPercentOff = (gto, delta) => {
    return  Math.abs(((delta / gto) * 100)).toFixed() >= 10;
}

function GTOTable({title, gtoTitles, gtoValues, playerValues}) {

    const hasPlayerValues = playerValues && playerValues.length > 0;

    return (
        <Fragment>
            <table className="styled-table">
                <thead><tr><th>{title}</th><th>GTO</th><th>YOU</th><th>&#916;</th></tr></thead>
                <tbody>
                {gtoTitles.map((title, i) => {
                    let playerValueClassName = '';
                    if (hasPlayerValues) {
                        playerValueClassName = isTenPercentOff(gtoValues[i], (playerValues[i] - gtoValues[i]).toFixed(1))
                            ? 'bad' : 'good';
                    }
                        return <tr key={i}>
                            <td><b>{title}</b></td>
                            <td>{gtoValues[i]}</td>
                            <td >{ hasPlayerValues ? playerValues[i] : '-' }</td>
                            <td className={playerValueClassName}>
                                { hasPlayerValues ? (playerValues[i] - gtoValues[i]).toFixed(1) : '-'}
                            </td>
                        </tr>;
                    }
                )}
                </tbody>
            </table>
        </Fragment>
    );
}

export default GTOTable;
