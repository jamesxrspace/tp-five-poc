import {
  Button,
  Center,
  FormControl,
  FormErrorMessage,
  FormLabel,
  Heading,
  Input,
  Stack,
} from '@chakra-ui/react';
import { SubmitHandler, useForm } from 'react-hook-form';
import { Navigate } from 'react-router-dom';
import { authing } from '@/constants/authing';
import { useAuth } from '@/hooks/useAuth';
import { AuthEvent, AuthState } from '@/machines/auth';
interface LoginFormValues {
  email: string;
  password: string;
}

const Login = () => {
  const [authState, authSend] = useAuth();
  const {
    register,
    formState: { errors },
    setError,
    handleSubmit,
  } = useForm<LoginFormValues>();

  if (authing) {
    authing.loginWithRedirect();
    return <div></div>;
  }
  if (authState.matches(AuthState.LOGGED_IN)) {
    return <Navigate to="/" />;
  }

  const onSubmit: SubmitHandler<LoginFormValues> = (data) => {
    const { email, password } = data;
    fetch(`${process.env.REACT_APP_MOCK_AUTH_URL}`, {
      method: 'POST',
      headers: {
        'Content-Type': 'application/x-www-form-urlencoded',
      },
      body: `grant_type=password&client_id=test_1234&email=${email}&password=${password}&scope=openid%20offline_access%20username%20profile%20email%20`,
    })
      .then((res: Response) => res.json())
      .then((data) => {
        if (data.err_code) {
          return Promise.reject(data);
        }
        authSend({ type: AuthEvent.COMPLETE_LOGIN, data: data.access_token });
      })
      .catch((err) => {
        console.error(err);
        setError('email', { message: err.msg });
        setError('password', { message: err.msg });
      });
  };

  return (
    <Center flexDirection="column" p={6} h="100vh">
      <Heading mb={8}>One CMS</Heading>
      <Stack gap={6} borderWidth={1} borderRadius="md" px={6} py={8} w="full" maxW="400px">
        <FormControl isInvalid={!!errors.email} isRequired>
          <FormLabel>Email</FormLabel>
          <Input type="email" {...register('email', { required: true })} />
          {errors.email && <FormErrorMessage>{errors.email.message}</FormErrorMessage>}
        </FormControl>
        <FormControl isInvalid={!!errors.password} isRequired>
          <FormLabel>Password</FormLabel>
          <Input type="password" {...register('password', { required: true })} />
          {errors.password && <FormErrorMessage>{errors.password.message}</FormErrorMessage>}
        </FormControl>
        <Button type="submit" colorScheme="purple" onClick={handleSubmit(onSubmit)}>
          Login
        </Button>
      </Stack>
    </Center>
  );
};
export default Login;
