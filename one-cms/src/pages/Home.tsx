import { Heading } from '@chakra-ui/react';
import styled from '@emotion/styled';
import logo from '@/logo.svg';
import { withTitle } from '@/utils/page';

const AppLogo = styled.img`
  height: 40vmin;
  pointer-events: none;
  margin: 0 auto;

  @media (prefers-reduced-motion: no-preference) {
    animation: App-logo-spin infinite 20s linear;
  }

  @keyframes App-logo-spin {
    from {
      transform: rotate(0deg);
    }
    to {
      transform: rotate(360deg);
    }
  }
`;

const Home = withTitle('navigation.common.home', () => {
  return (
    <div>
      <AppLogo src={logo} className="App-logo" alt="logo" />
      <Heading>Home</Heading>
    </div>
  );
});

export default Home;
