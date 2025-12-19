import { useEffect, useState } from 'react'
import { doctorsApi, departmentsApi } from '../services/api'
import type { Doctor, Department } from '../types'
import './Common.css'

export default function Doctors() {
  const [doctors, setDoctors] = useState<Doctor[]>([])
  const [departments, setDepartments] = useState<Department[]>([])
  const [loading, setLoading] = useState(true)
  const [showModal, setShowModal] = useState(false)
  const [editingDoctor, setEditingDoctor] = useState<Doctor | null>(null)
  const [formData, setFormData] = useState<Doctor>({
    name: '',
    specialization: '',
    consultationFee: 0,
    email: '',
    phone: '',
    departmentId: 0,
  })

  useEffect(() => {
    loadData()
  }, [])

  const loadData = async () => {
    try {
      const [doctorsData, departmentsData] = await Promise.all([
        doctorsApi.getAll(),
        departmentsApi.getAll()
      ])
      setDoctors(doctorsData)
      setDepartments(departmentsData)
    } catch (error) {
      console.error('Error loading data:', error)
    } finally {
      setLoading(false)
    }
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    try {
      if (editingDoctor?.doctorId) {
        await doctorsApi.update(editingDoctor.doctorId, formData)
      } else {
        await doctorsApi.create(formData)
      }
      setShowModal(false)
      setEditingDoctor(null)
      setFormData({ name: '', specialization: '', consultationFee: 0, email: '', phone: '', departmentId: 0 })
      loadData()
    } catch (error) {
      console.error('Error saving doctor:', error)
      alert('Error saving doctor')
    }
  }

  const handleEdit = (doctor: Doctor) => {
    setEditingDoctor(doctor)
    setFormData(doctor)
    setShowModal(true)
  }

  const handleDelete = async (id: number) => {
    if (!confirm('Are you sure you want to delete this doctor?')) return
    try {
      await doctorsApi.delete(id)
      loadData()
    } catch (error) {
      console.error('Error deleting doctor:', error)
      alert('Error deleting doctor')
    }
  }

  if (loading) return <div>Loading...</div>

  return (
    <div className="page-container">
      <div className="page-header">
        <h1>Mjekët</h1>
        <button onClick={() => { setShowModal(true); setEditingDoctor(null); setFormData({ name: '', specialization: '', consultationFee: 0, email: '', phone: '', departmentId: departments[0]?.departmentId || 0 }) }} className="btn-primary">
          + Shto Mjek
        </button>
      </div>

      <table className="data-table">
        <thead>
          <tr>
            <th>ID</th>
            <th>Emri</th>
            <th>Specializimi</th>
            <th>Tarifa</th>
            <th>Email</th>
            <th>Telefon</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {doctors.map((doctor) => (
            <tr key={doctor.doctorId}>
              <td>{doctor.doctorId}</td>
              <td>{doctor.name}</td>
              <td>{doctor.specialization}</td>
              <td>{doctor.consultationFee} ALL</td>
              <td>{doctor.email || '-'}</td>
              <td>{doctor.phone || '-'}</td>
              <td>
                <button onClick={() => handleEdit(doctor)} className="btn-edit">Edit</button>
                <button onClick={() => handleDelete(doctor.doctorId!)} className="btn-delete">Delete</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      {showModal && (
        <div className="modal-overlay" onClick={() => setShowModal(false)}>
          <div className="modal-content" onClick={(e) => e.stopPropagation()}>
            <h2>{editingDoctor ? 'Edit Doctor' : 'New Doctor'}</h2>
            <form onSubmit={handleSubmit}>
              <div className="form-group">
                <label>Name *</label>
                <input type="text" value={formData.name} onChange={(e) => setFormData({ ...formData, name: e.target.value })} required />
              </div>
              <div className="form-group">
                <label>Specialization *</label>
                <input type="text" value={formData.specialization} onChange={(e) => setFormData({ ...formData, specialization: e.target.value })} required />
              </div>
              <div className="form-group">
                <label>Consultation Fee *</label>
                <input type="number" value={formData.consultationFee} onChange={(e) => setFormData({ ...formData, consultationFee: parseInt(e.target.value) })} required />
              </div>
              <div className="form-group">
                <label>Department *</label>
                <select value={formData.departmentId} onChange={(e) => setFormData({ ...formData, departmentId: parseInt(e.target.value) })} required>
                  <option value="">Select Department</option>
                  {departments.map(dept => <option key={dept.departmentId} value={dept.departmentId}>{dept.name}</option>)}
                </select>
              </div>
              <div className="form-group">
                <label>Email</label>
                <input type="email" value={formData.email} onChange={(e) => setFormData({ ...formData, email: e.target.value })} />
              </div>
              <div className="form-group">
                <label>Phone</label>
                <input type="text" value={formData.phone} onChange={(e) => setFormData({ ...formData, phone: e.target.value })} />
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

