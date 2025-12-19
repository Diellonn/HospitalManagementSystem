import { Outlet, Link, useNavigate } from 'react-router-dom'
import { useAuth } from '../context/AuthContext'
import './Layout.css'

export default function Layout() {
  const { user, logout } = useAuth()
  const navigate = useNavigate()

  const handleLogout = () => {
    logout()
    navigate('/login')
  }

  return (
    <div className="layout">
      <nav className="sidebar">
        <div className="logo">
          <h2>🏥 HMS</h2>
        </div>
        <ul className="nav-menu">
          <li>
            <Link to="/">Dashboard</Link>
          </li>
          {user?.role === 'Admin' && (
            <>
              <li>
                <Link to="/users">Përdoruesit</Link>
              </li>
              <li>
                <Link to="/patients">Pacientët</Link>
              </li>
              <li>
                <Link to="/doctors">Mjekët</Link>
              </li>
              <li>
                <Link to="/departments">Departamentet</Link>
              </li>
            </>
          )}
          {user?.role === 'Doctor' && (
            <>
              <li>
                <Link to="/medical-records">Rekordet Mjekësore</Link>
              </li>
            </>
          )}
          <li>
            <Link to="/appointments">Takimet</Link>
          </li>
          {user?.role === 'Patient' && (
            <li>
              <Link to="/diagnosis">Diagnoza</Link>
            </li>
          )}
          <li>
            <Link to="/invoices">Faturat</Link>
          </li>
        </ul>
        <div className="user-info">
          <div className="user-details">
            <p><strong>{user?.username}</strong></p>
            <p className="role">{user?.role}</p>
          </div>
          <button onClick={handleLogout} className="logout-btn">
            Dil
          </button>
        </div>
      </nav>
      <main className="main-content">
        <Outlet />
      </main>
    </div>
  )
}

