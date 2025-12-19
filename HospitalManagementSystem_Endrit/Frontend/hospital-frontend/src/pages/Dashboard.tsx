import { useEffect, useState } from 'react'
import { patientsApi, doctorsApi, appointmentsApi, invoicesApi } from '../services/api'
import { useAuth } from '../context/AuthContext'
import './Dashboard.css'

export default function Dashboard() {
  const { user } = useAuth()
  const [stats, setStats] = useState({
    patients: 0,
    doctors: 0,
    appointments: 0,
    invoices: 0
  })
  const [loading, setLoading] = useState(true)

  useEffect(() => {
    const fetchStats = async () => {
      try {
        const isPatient = user?.role === 'Patient'
        
        if (isPatient) {
          // For patients, show only their own stats
          const patientsData = await patientsApi.getAll()
          const foundPatient = patientsData.find(p => p.email === user?.email || p.name === user?.username)
          
          if (foundPatient) {
            const [appointments, invoices] = await Promise.all([
              appointmentsApi.getByPatient(foundPatient.patientId!),
              invoicesApi.getByPatient(foundPatient.patientId!)
            ])
            setStats({
              patients: 0,
              doctors: 0,
              appointments: appointments.length,
              invoices: invoices.length
            })
          }
        } else {
          // For admins, show all stats
          const [patients, doctors, appointments, invoices] = await Promise.all([
            patientsApi.getAll(),
            doctorsApi.getAll(),
            appointmentsApi.getAll(),
            invoicesApi.getAll()
          ])
          setStats({
            patients: patients.length,
            doctors: doctors.length,
            appointments: appointments.length,
            invoices: invoices.length
          })
        }
      } catch (error) {
        console.error('Error fetching stats:', error)
      } finally {
        setLoading(false)
      }
    }
    fetchStats()
  }, [user])

  if (loading) {
    return <div>Loading...</div>
  }

  const isPatient = user?.role === 'Patient'

  return (
    <div className="dashboard">
      <h1>Dashboard</h1>
      <div className="stats-grid">
        {!isPatient && (
          <>
            <div className="stat-card">
              <div className="stat-icon">👥</div>
              <div className="stat-info">
                <h3>Pacientët</h3>
                <p className="stat-number">{stats.patients}</p>
              </div>
            </div>
            <div className="stat-card">
              <div className="stat-icon">👨‍⚕️</div>
              <div className="stat-info">
                <h3>Mjekët</h3>
                <p className="stat-number">{stats.doctors}</p>
              </div>
            </div>
          </>
        )}
        <div className="stat-card">
          <div className="stat-icon">📅</div>
          <div className="stat-info">
            <h3>{isPatient ? 'Takimet e Mia' : 'Takimet'}</h3>
            <p className="stat-number">{stats.appointments}</p>
          </div>
        </div>
        <div className="stat-card">
          <div className="stat-icon">💰</div>
          <div className="stat-info">
            <h3>{isPatient ? 'Faturat e Mia' : 'Faturat'}</h3>
            <p className="stat-number">{stats.invoices}</p>
          </div>
        </div>
      </div>
    </div>
  )
}

