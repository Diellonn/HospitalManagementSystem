import { useEffect, useState } from 'react'
import { departmentsApi } from '../services/api'
import type { Department } from '../types'
import './Common.css'

export default function Departments() {
  const [departments, setDepartments] = useState<Department[]>([])
  const [loading, setLoading] = useState(true)
  const [showModal, setShowModal] = useState(false)
  const [editingDept, setEditingDept] = useState<Department | null>(null)
  const [formData, setFormData] = useState({ name: '' })

  useEffect(() => {
    loadData()
  }, [])

  const loadData = async () => {
    try {
      const data = await departmentsApi.getAll()
      setDepartments(data)
    } catch (error) {
      console.error('Error loading departments:', error)
    } finally {
      setLoading(false)
    }
  }

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    try {
      if (editingDept?.departmentId) {
        await departmentsApi.update(editingDept.departmentId, formData)
      } else {
        await departmentsApi.create(formData)
      }
      setShowModal(false)
      setEditingDept(null)
      setFormData({ name: '' })
      loadData()
    } catch (error) {
      console.error('Error saving department:', error)
      alert('Error saving department')
    }
  }

  const handleEdit = (dept: Department) => {
    setEditingDept(dept)
    setFormData({ name: dept.name })
    setShowModal(true)
  }

  const handleDelete = async (id: number) => {
    if (!confirm('Are you sure you want to delete this department?')) return
    try {
      await departmentsApi.delete(id)
      loadData()
    } catch (error) {
      console.error('Error deleting department:', error)
      alert('Error deleting department')
    }
  }

  if (loading) return <div>Loading...</div>

  return (
    <div className="page-container">
      <div className="page-header">
        <h1>Departamentet</h1>
        <button onClick={() => { setShowModal(true); setEditingDept(null); setFormData({ name: '' }) }} className="btn-primary">
          + Shto Departament
        </button>
      </div>

      <table className="data-table">
        <thead>
          <tr>
            <th>ID</th>
            <th>Emri</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {departments.map((dept) => (
            <tr key={dept.departmentId}>
              <td>{dept.departmentId}</td>
              <td>{dept.name}</td>
              <td>
                <button onClick={() => handleEdit(dept)} className="btn-edit">Edit</button>
                <button onClick={() => handleDelete(dept.departmentId!)} className="btn-delete">Delete</button>
              </td>
            </tr>
          ))}
        </tbody>
      </table>

      {showModal && (
        <div className="modal-overlay" onClick={() => setShowModal(false)}>
          <div className="modal-content" onClick={(e) => e.stopPropagation()}>
            <h2>{editingDept ? 'Edit Department' : 'New Department'}</h2>
            <form onSubmit={handleSubmit}>
              <div className="form-group">
                <label>Name *</label>
                <input type="text" value={formData.name} onChange={(e) => setFormData({ name: e.target.value })} required />
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

