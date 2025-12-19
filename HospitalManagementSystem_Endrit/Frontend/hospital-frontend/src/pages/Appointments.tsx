import { useEffect, useState } from 'react'
import { appointmentsApi, patientsApi, doctorsApi } from '../services/api'
import { useAuth } from '../context/AuthContext'
import type { Appointment, Patient, Doctor } from '../types'
import './Common.css'

export default function Appointments() {
  const { user } = useAuth()
  const [appointments, setAppointments] = useState<Appointment[]>([])
  const [patients, setPatients] = useState<Patient[]>([])
  const [doctors, setDoctors] = useState<Doctor[]>([])
  const [currentPatientId, setCurrentPatientId] = useState<number | null>(null)
  const [loading, setLoading] = useState(true)
  const [showModal, setShowModal] = useState(false)
  const [formData, setFormData] = useState({
    patientId: 0,
    doctorId: 0,
    time: '',
    reason: '',
  })

  useEffect(() => {
    loadData()
  }, [])

  const loadData = async () => {
    try {
      const isPatient = user?.role === 'Patient'
      
      if (isPatient) {
        // For patients, find their patient record and load only their appointments
        const patientsData = await patientsApi.getAll()
        const foundPatient = patientsData.find(p => p.email === user?.email || p.name === user?.username)
        
        if (foundPatient) {
          setCurrentPatientId(foundPatient.patientId!)
          const appointmentsData = await appointmentsApi.getByPatient(foundPatient.patientId!)
          setAppointments(appointmentsData)
        }
        const doctorsData = await doctorsApi.getAll()
        setDoctors(doctorsData)
      } else {
        // For admins, load all appointments
        const [appointmentsData, patientsData, doctorsData] = await Promise.all([
          appointmentsApi.getAll(),
          patientsApi.getAll(),
          doctorsApi.getAll()
        ])
        setAppointments(appointmentsData)
        setPatients(patientsData)
        setDoctors(doctorsData)
      }
    } catch (error) {
      console.error('Error loading data:', error)
    } finally {
      setLoading(false)
    }
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    try {
      await appointmentsApi.create(formData)
      setShowModal(false)
      setFormData({ patientId: 0, doctorId: 0, time: '', reason: '' })
      loadData()
    } catch (error: any) {
      console.error('Error creating appointment:', error)
      alert(error.response?.data?.message || 'Error creating appointment')
    }
  }

  const handleCancel = async (id: number) => {
    if (!window.confirm('Are you sure you want to cancel this appointment?')) return
    try {
      await appointmentsApi.cancel(id)
      loadData()
    } catch (error) {
      console.error('Error canceling appointment:', error)
      alert('Error canceling appointment')
    }
  }

  if (loading) return <div>Loading...</div>

  const isPatient = user?.role === 'Patient'

  return (
    <div className="page-container">
      <div className="page-header">
        <h1>Takimet</h1>
        {!isPatient && (
          <button
            onClick={() => {
              setShowModal(true)
              setFormData({ patientId: 0, doctorId: 0, time: '', reason: '' })
            }}
            className="btn-primary"
          >
            + Shto Takim
          </button>
        )}
        {isPatient && currentPatientId && (
          <button
            onClick={() => {
              setShowModal(true)
              setFormData({ patientId: currentPatientId, doctorId: 0, time: '', reason: '' })
            }}
            className="btn-primary"
          >
            + Rezervo Takim
          </button>
        )}
      </div>

      <table className="data-table">
        <thead>
          <tr>
            <th>ID</th>
            <th>Pacienti</th>
            <th>Mjeku</th>
            <th>Koha</th>
            <th>Arsyeja</th>
            <th>Statusi</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {appointments.map((appointment) => (
            <tr key={appointment.appointmentId}>
              <td>{appointment.appointmentId}</td>
              <td>{appointment.patientName || appointment.patient?.name || `Patient ${appointment.patientId}`}</td>
              <td>{appointment.doctorName || appointment.doctor?.name || `Doctor ${appointment.doctorId}`}</td>
              <td>{new Date(appointment.time).toLocaleString()}</td>
              <td>{appointment.reason}</td>
              <td>
                <span className={`status-badge status-${appointment.status.toLowerCase()}`}>
                  {appointment.status}
                </span>
              </td>
              <td>
                {appointment.status === 'Scheduled' && (
                  <button
                    onClick={() => handleCancel(appointment.appointmentId!)}
                    className="btn-delete"
                  >
                    Cancel
                  </button>
                )}
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      {showModal && (
        <div className="modal-overlay" onClick={() => setShowModal(false)}>
          <div className="modal-content" onClick={(e) => e.stopPropagation()}>
            <h2>New Appointment</h2>
            <form onSubmit={handleSubmit}>
              {!isPatient && (
                <div className="form-group">
                  <label>Patient *</label>
                  <select
                    value={formData.patientId}
                    onChange={(e) => setFormData({ ...formData, patientId: parseInt(e.target.value) })}
                    required
                  >
                    <option value={0}>Select Patient</option>
                    {patients.map((patient) => (
                      <option key={patient.patientId} value={patient.patientId}>
                        {patient.name}
                      </option>
                    ))}
                  </select>
                </div>
              )}

              <div className="form-group">
                <label>Doctor *</label>
                <select
                  value={formData.doctorId}
                  onChange={(e) => setFormData({ ...formData, doctorId: parseInt(e.target.value) })}
                  required
                >
                  <option value={0}>Select Doctor</option>
                  {doctors.map((doctor) => (
                    <option key={doctor.doctorId} value={doctor.doctorId}>
                      {doctor.name} - {doctor.specialization}
                    </option>
                  ))}
                </select>
              </div>

              <div className="form-group">
                <label>Time *</label>
                <input
                  type="datetime-local"
                  value={formData.time}
                  onChange={(e) => setFormData({ ...formData, time: e.target.value })}
                  required
                />
              </div>

              <div className="form-group">
                <label>Reason *</label>
                <textarea
                  value={formData.reason}
                  onChange={(e) => setFormData({ ...formData, reason: e.target.value })}
                  required
                  rows={3}
                  style={{ width: '100%', padding: '10px', border: '1px solid #ddd', borderRadius: '5px' }}
                />
              </div>

              <div className="form-actions">
                <button type="button" onClick={() => setShowModal(false)} className="btn-secondary">
                  Cancel
                </button>
                <button type="submit" className="btn-primary">
                  Create
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  )
}
