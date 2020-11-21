import React from 'react';
import { render, fireEvent, screen } from '@testing-library/react';
import GameSearchInput from '../GameSearchInput';

describe('Game Search Input', () => {
  it('should match the snapshot', () => {
    const { container } = render(<GameSearchInput onGameChange={jest.fn()} />);

    expect(container.firstChild).toMatchSnapshot();
  });

  it('should trigger the game change event when the user presses the search button', () => {
    const onGameChange = jest.fn();

    render(<GameSearchInput onGameChange={onGameChange} />);

    const searchInput = screen.getByPlaceholderText('Search');
    const searchButton = screen.getByRole('button');
    
    fireEvent.change(searchInput, { target: { value: 'test Game' } });
    fireEvent.click(searchButton, { button: 1 })

    expect(onGameChange).toHaveBeenCalledWith('test Game');
  });

  it('should trigger the game change event when the user presses the enter/return key', () => {
    const onGameChange = jest.fn();

    render(<GameSearchInput onGameChange={onGameChange} />);

    const searchInput = screen.getByPlaceholderText('Search');
    
    fireEvent.change(searchInput, { target: { value: 'test Game' } });
    fireEvent.keyDown(searchInput, { key: 'enter', keyCode: 13 });

    expect(onGameChange).toHaveBeenCalledWith('test Game');
  });
})
