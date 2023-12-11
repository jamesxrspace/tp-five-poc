import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import IconButtonWithTooltip from './IconButtonWithTooltip';

describe('IconButtonWithTooltip', () => {
  it('should render the button correctly', () => {
    const mockClick = jest.fn();

    render(
      <IconButtonWithTooltip
        label="Tooltip Test"
        aria-label="Button Test"
        icon={<span>Icon</span>}
        onClick={mockClick}
      />,
    );

    const button = screen.getByRole('button');
    fireEvent.click(button);

    expect(button).toBeInTheDocument();
    expect(mockClick).toHaveBeenCalledTimes(1);
  });

  it('should disable the button when "isDisabled" prop is true', () => {
    render(
      <IconButtonWithTooltip
        label="Tooltip Test"
        aria-label="Button Test"
        icon={<span>Icon</span>}
        isDisabled={true}
      />,
    );

    const button = screen.getByRole('button');
    expect(button).toBeDisabled();
  });

  it('should show and hide tooltip label correctly when user interacted with the button', async () => {
    render(
      <IconButtonWithTooltip
        label="Tooltip Label"
        aria-label="Button Test"
        icon={<span>MockIcon</span>}
      />,
    );

    const iconElement = screen.getByText('MockIcon');

    fireEvent.pointerOver(iconElement);
    await screen.findByRole('tooltip');

    expect(screen.getByText('Tooltip Label')).toBeInTheDocument();

    if (iconElement && iconElement.parentElement) {
      fireEvent.pointerLeave(iconElement.parentElement);
    }

    await waitFor(() => {
      expect(screen.queryByText('Tooltip Label')).not.toBeInTheDocument();
    });
  });
});
