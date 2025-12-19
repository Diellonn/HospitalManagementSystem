import { useEffect, useState } from 'react'
import { patientsApi, doctorsApi } from '../services/api'
import type { Patient, Doctor } from '../types'
import './Common.css'

export default function Patients() {
  const [patients, setPatients] = useState<Patient[]>([])
  const [doctors, setDoctors] = useState<Doctor[]>([])
  const [loading, setLoading] = useState(true)
  const [showModal, setShowModal] = useState(false)
  const [editingPatient, setEditingPatient] = useState<Patient | null>(null)
  const [formData, setFormData] = useState<Patient>({
    name: '',
    age: 0,
    insurance: '',
    address: '',
    phone: '',
    email: '',
    dateOfBirth: '',
  })

  useEffect(() => {
    loadData()
  }, [])

  const loadData = async () => {
    try {
      const [patientsData, doctorsData] = await Promise.all([
        patientsApi.getAll(),
        doctorsApi.getAll()
      ])
      setPatients(patientsData)
      setDoctors(doctorsData)
    } catch (error) {
      console.error('Error loading data:', error)
    } finally {
      setLoading(false)
    }
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    try {
      if (editingPatient?.patientId) {
        await patientsApi.update(editingPatient.patientId, formData)
      } else {
        await patientsApi.create(formData)
      }
      setShowModal(false)
      setEditingPatient(null)
      setFormData({ name: '', age: 0, insurance: '', address: '', phone: '', email: '', dateOfBirth: '' })
      loadData()
    } catch (error) {
      console.error('Error saving patient:', error)
      alert('Error saving patient')
    }
  }

  const handleEdit = (patient: Patient) => {
    setEditingPatient(patient)
    setFormData(patient)
    setShowModal(true)
  }

  const handleDelete = async (id: number) => {
    if (!confirm('Are you sure you want to delete this patient?')) return
    try {
      await patientsApi.delete(id)
      loadData()
    } catch (error) {
      console.error('Error deleting patient:', error)
      alert('Error deleting patient')
    }
  }

  if (loading) return <div>Loading...</div>

  return (
    <div className="page-container">
      <div className="page-header">
        <h1>Pacientët</h1>
        <button onClick={() => { setShowModal(true); setEditingPatient(null); setFormData({ name: '', age: 0, insurance: '', address: '', phone: '', email: '', dateOfBirth: '' }) }} className="btn-primary">
          + Shto Pacient
        </button>
      </div>

      <table className="data-table">
        <thead>
          <tr>
            <th>ID</th>
            <th>Emri</th>
            <th>Mosha</th>
            <th>Sigurimi</th>
            <th>Telefon</th>
            <th>Email</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {patients.map((patient) => (
            <tr key={patient.patientId}>
              <td>{patient.patientId}</td>
              <td>{patient.name}</td>
              <td>{patient.age}</td>
              <td>{patient.insurance}</td>
              <td>{patient.phone || '-'}</td>
              <td>{patient.email || '-'}</td>
              <td>
                <button onClick={() => handleEdit(patient)} className="btn-edit">Edit</button>
                <button onClick={() => handleDelete(patient.patientId!)} className="btn-delete">Delete</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      {showModal && (
        <div className="modal-overlay" onClick={() => setShowModal(false)}>
          <div className="modal-content" onClick={(e) => e.stopPropagation()}>
            <h2>{editingPatient ? 'Edit Patient' : 'New Patient'}</h2>
            <form onSubmit={handleSubmit}>
              <div className="form-group">
                <label>Name *</label>
                <input type="text" value={formData.name} onChange={(e) => setFormData({ ...formData, name: e.target.value })} required />
              </div>
              <div className="form-group">
                <label>Age *</label>
                <input type="number" value={formData.age} onChange={(e) => setFormData({ ...formData, age: parseInt(e.target.value) })} required />
              </div>
              <div className="form-group">
                <label>Insurance *</label>
                <input type="text" value={formData.insurance} onChange={(e) => setFormData({ ...formData, insurance: e.target.value })} required />
              </div>
              <div className="form-group">
                <label>Address</label>
                <input type="text" value={formData.address} onChange={(e) => setFormData({ ...formData, address: e.target.value })} />
              </div>
              <div className="form-group">
                <label>Phone</label>
                <input type="text" value={formData.phone} onChange={(e) => setFormData({ ...formData, phone: e.target.value })} />
              </div>
              <div className="form-group">
                <label>Email</label>
                <input type="email" value={formData.email} onChange={(e) => setFormData({ ...formData, email: e.target.value })} />
              </div>
              <div className="form-group">
                <label>Date of Birth</label>
                <input type="date" value={formData.dateOfBirth} onChange={(e) => setFormData({ ...formData, dateOfBirth: e.target.value })} />
              </div>
              <div className="form-actions">
                <button type="button" onClick={() => setShowModal(false)} className="btn-secondary">Cancel</button>
                <button type="submit" className="btn-primary">Save</button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  )
}

