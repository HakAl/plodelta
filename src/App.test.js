// Copyright (c) 2025 HakAl.  See LICENCES/MIT.txt for licence terms.
import { render, screen } from '@testing-library/react';
import App from './App';

test('renders header', () => {
    render(<App />);
    // todo
    // get <h1> with text GTO Coach and assert presence
    // const heading = screen.getByRole('heading', { name /i/i });
    // expect(heading).toBeInTheDocument();
});

test('loads sample CSV button', () => {
    render(<App />);
    const button = screen.getByRole('button', { name: /load sample/i });
    expect(button).toBeInTheDocument();
});