import { GoogleLogin } from '@react-oauth/google'
import axios from 'axios'

function App() {
  return (
    <main>
      <h1>TravelDNA</h1>

      <GoogleLogin
        onSuccess={async (response) => {
          if (!response.credential) {
            return
          }

          const result = await axios.post('https://localhost:7102/api/auth/google',
            {
              idToken: response.credential
            }
          )

          localStorage.setItem('accessToken', result.data.accessToken)
        }}
        onError={() => {
          console.error('Google Login Failed')
        }}
      />

    </main>
  )
}

export default App
