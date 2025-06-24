import React, { useState } from 'react';
import axios from 'axios';  // Import axios
import './LoginAndRegis.css';

const LoginAndRegis = () => {
  const [isSignup, setIsSignup] = useState(false);
  const [isLogin, setIsLogin] = useState(true);

  // Login form states
  const [loginUserName, setLoginUserName] = useState('');
  const [loginPassword, setLoginPassword] = useState('');

  // Signup form states
  const [signupUserName, setSignupUserName] = useState('');
  const [signupPassword, setSignupPassword] = useState('');
  const [signupEmail, setSignupEmail] = useState('');
  const [signupAge, setSignupAge] = useState('');
  
  const [errorMessage, setErrorMessage] = useState('');

  // Hàm xử lý khi nhấn "Signup"
  const handleSignupClick = () => {
    setIsSignup(true);
    setIsLogin(false);
  };

  // Hàm xử lý khi nhấn "Login"
  const handleLoginClick = () => {
    setIsSignup(false);
    setIsLogin(true);
  };

  // Hàm gọi API khi nhấn nút Login
  const handleLoginSubmit = async (e) => {
    e.preventDefault();

    if (!loginUserName || !loginPassword) {
        setErrorMessage('Please enter both username and password.');
        return;
    }

    try {
        const response = await axios.post('http://localhost:5285/api/account/login', {
            userName: loginUserName,
            password: loginPassword,
        });

        alert('Login successful!');
        setErrorMessage('');
    } catch (error) {
        if (error.response) {
            // Kiểm tra mã lỗi HTTP từ API
            if (error.response.status === 401) {
                alert(error.response.data);
                setErrorMessage(error.response.data.message || 'Login failed. Please try again.');
            } else {
                setErrorMessage(`Error: ${error.response.status} - ${error.response.data}`);
            }
        } else {
            setErrorMessage('An error occurred while logging in.');
        }
        console.error('Error:', error);
    }
};

  // Hàm gọi API khi nhấn nút Signup
  const handleSignupSubmit = async (e) => {
    e.preventDefault();

    // Kiểm tra nếu có trường thông tin bị bỏ trống
    if (!signupUserName || !signupPassword || !signupEmail || !signupAge) {
      setErrorMessage('Please fill in all fields.');
      return;
    }

    try {
      const response = await axios.post('http://localhost:5285/api/account/register', {
        userName: signupUserName,
        password: signupPassword,
        emailAddress: signupEmail,
        age: signupAge,
      });

      if (response.status === 200) {
        alert('Signup successful! Please login.');
        setIsSignup(false);
        setIsLogin(true);
    } else {
      setErrorMessage(response.data.message || 'Signup failed. Please try again.');
    }
    } catch (error) {
      setErrorMessage('An error occurred while signing up.');
      console.error('Error:', error);
    }
  };

  return (
  //   <div className="wrapper">
  //     <div className="title-text">
  //       <div className={`title login active`} onClick={handleLoginClick}>
  //         {isLogin ? "Login Form" : "Signup Form"}
  //       </div>

  //       <div className={`title signup active`} onClick={handleSignupClick}>
  //         {isLogin ? "Signup Form" : "Login Form"}
  //       </div>
  //     </div>
  //     <div className="form-container">
  //       <div className="slide-controls">
  //         <input
  //           type="radio"
  //           name="slide"
  //           id="login"
  //           checked={isLogin}
  //           onChange={handleLoginClick}
  //         />
  //         <input
  //           type="radio"
  //           name="slide"
  //           id="signup"
  //           checked={isSignup}
  //           onChange={handleSignupClick}
  //         />
  //         <label htmlFor="login" className="slide login" onClick={handleLoginClick}>
  //           Login
  //         </label>
  //         <label htmlFor="signup" className="slide signup" onClick={handleSignupClick}>
  //           Signup
  //         </label>
  //         <div className="slider-tab" style={{ left: isSignup ? '50%' : '0%' }}></div>
  //       </div>
  //       <div className="form-inner">
  //         {/* Login Form */}
  //         <form className={`login ${isLogin ? 'active' : ''}`} onSubmit={handleLoginSubmit}>
  //           <div className="field">
  //             <input
  //               type="text"
  //               placeholder="UserName"
  //               required
  //               value={loginUserName}
  //               onChange={(e) => setLoginUserName(e.target.value)}
  //             />
  //           </div>
  //           <div className="field">
  //             <input
  //               type="password"
  //               placeholder="Password"
  //               required
  //               value={loginPassword}
  //               onChange={(e) => setLoginPassword(e.target.value)}
  //             />
  //           </div>
  //           <div className="field btn">
  //             <div className="btn-layer"></div>
  //             <input type="submit" value="Login" />
  //           </div>
  //           {errorMessage && <div className="error-message">{errorMessage}</div>}
  //           <div className="signup-link">
  //             Not a member? <a href="#" onClick={handleSignupClick}>Signup now</a>
  //           </div>
  //         </form>
          
  //         {/* Signup Form */}
  //         <form className={`signup ${isSignup ? 'active' : ''}`} onSubmit={handleSignupSubmit}>
  //           <div className="field">
  //             <input
  //               type="text"
  //               placeholder="UserName"
  //               required
  //               value={signupUserName}
  //               onChange={(e) => setSignupUserName(e.target.value)}
  //             />
  //           </div>
  //           <div className="field">
  //             <input
  //               type="password"
  //               placeholder="Password"
  //               required
  //               value={signupPassword}
  //               onChange={(e) => setSignupPassword(e.target.value)}
  //             />
  //           </div>
  //           <div className="field">
  //             <input
  //               type="email"
  //               placeholder="Email Address"
  //               required
  //               value={signupEmail}
  //               onChange={(e) => setSignupEmail(e.target.value)}
  //             />
  //           </div>
  //           <div className="field">
  //             <input
  //               type="number"
  //               placeholder="Age"
  //               required
  //               value={signupAge}
  //               onChange={(e) => setSignupAge(e.target.value)}
  //             />
  //           </div>
  //           <div className="field btn">
  //             <div className="btn-layer"></div>
  //             <input type="submit" value="Signup" />
  //           </div>
  //         </form>
  //       </div>
  //     </div>
  //   </div>
  // );
  <div className="flex justify-center items-center min-h-screen bg-gradient-to-br from-blue-500 to-purple-600">
      <div className="bg-white p-6 rounded-lg shadow-xl w-96">
        <div className="flex justify-around mb-6">
          <button
            className={`px-4 py-2 font-semibold rounded-md transition-all ${isLogin ? 'bg-blue-500 text-white' : 'bg-gray-200'}`}
            onClick={handleLoginClick}
          >
            Login
          </button>
          <button
            className={`px-4 py-2 font-semibold rounded-md transition-all ${!isLogin ? 'bg-blue-500 text-white' : 'bg-gray-200'}`}
            onClick={handleSignupClick}
          >
            Signup
          </button>
        </div>

        {isLogin ? (
          <form onSubmit={handleLoginSubmit} className="space-y-4">
            <input
              type="text"
              placeholder="Username"
              required
              className="w-full p-2 border rounded-md"
              value={loginUserName}
              onChange={(e) => setLoginUserName(e.target.value)}
            />
            <input
              type="password"
              placeholder="Password"
              required
              className="w-full p-2 border rounded-md"
              value={loginPassword}
              onChange={(e) => setLoginPassword(e.target.value)}
            />
            <button type="submit" className="w-full py-2 bg-blue-500 text-white rounded-md hover:bg-blue-600">Login</button>
            {errorMessage && <div className="text-red-500 text-sm">{errorMessage}</div>}
            <p className="text-sm text-center">Not a member? <span className="text-blue-500 cursor-pointer" onClick={handleSignupClick}>Signup now</span></p>
          </form>
        ) : (
          <form onSubmit={handleSignupSubmit} className="space-y-4">
            <input
              type="text"
              placeholder="Username"
              required
              className="w-full p-2 border rounded-md"
              value={signupUserName}
              onChange={(e) => setSignupUserName(e.target.value)}
            />
            <input
              type="password"
              placeholder="Password"
              required
              className="w-full p-2 border rounded-md"
              value={signupPassword}
              onChange={(e) => setSignupPassword(e.target.value)}
            />
            <input
              type="email"
              placeholder="Email Address"
              required
              className="w-full p-2 border rounded-md"
              value={signupEmail}
              onChange={(e) => setSignupEmail(e.target.value)}
            />
            <input
              type="number"
              placeholder="Age"
              required
              className="w-full p-2 border rounded-md"
              value={signupAge}
              onChange={(e) => setSignupAge(e.target.value)}
            />
            <button type="submit" className="w-full py-2 bg-blue-500 text-white rounded-md hover:bg-blue-600">Signup</button>
          </form>
        )}
      </div>
    </div>
  );
};

export default LoginAndRegis;
