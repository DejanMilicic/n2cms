using System;
using NHibernate;
using System.Data;
using System.Diagnostics;

namespace N2.Persistence.NH
{
	public class NHTransaction : ITransaction
	{
		NHibernate.ITransaction transaction;
		private SessionContext context;
		bool isOriginator = true;

		public bool IsCommitted { get; set; }
		public bool IsRollbacked { get; set; }

		public NHTransaction(ISessionProvider sessionProvider)
		{
			Debug.WriteLine("Transaction {");
			Debug.Indent();

			context = sessionProvider.OpenSession;
		    
			ISession session = context.Session;
			transaction = session.Transaction;
			if (transaction.IsActive)
				isOriginator = false; // The method that first opened the transaction should also close it
			else if (sessionProvider.Isolation.HasValue)
				transaction.Begin(sessionProvider.Isolation.Value);
			else
                transaction.Begin();

			if (context.Transaction != null)
			{
				context.Transaction.Committed += (o, s) => OnCommit();
				context.Transaction.Disposed += (o, s) => OnDispose();
				context.Transaction.Rollbacked += (o, s) => OnRollback();
			}
		}

		#region ITransaction Members

		/// <summary>Commits the transaction.</summary>
		public void Commit()
		{
			if (isOriginator && !transaction.WasCommitted && !transaction.WasRolledBack)
			{
				Debug.WriteLine("Commit");
				transaction.Commit();
				IsCommitted = true;
				RemoveFromContext();

				OnCommit();
			}
		}

		private void OnCommit()
		{
			if (Committed != null)
				Committed(this, new EventArgs());
		}

		/// <summary>Rollsbacks the transaction</summary>
		public void Rollback()
		{
			if (!transaction.WasCommitted && !transaction.WasRolledBack)
			{
				Debug.WriteLine("Rollback");
				transaction.Rollback();
				IsRollbacked = true;
				RemoveFromContext();

				OnRollback();
			}
		}

		private void OnRollback()
		{
			if (Rollbacked != null)
				Rollbacked(this, new EventArgs());
		}

		#endregion

		#region IDisposable Members

        public void Dispose()
		{
			Debug.Unindent();
			Debug.WriteLine("}");
			if (isOriginator)
			{
				Rollback();
				transaction.Dispose();
				RemoveFromContext();
			}
			OnDispose();
		}

		private void OnDispose()
		{
			if (Disposed != null)
				Disposed(this, new EventArgs());
		}

		#endregion

		private void RemoveFromContext()
		{
			if (object.ReferenceEquals(context.Transaction, this))
				context.Transaction = null;
		}

		/// <summary>Invoked after the transaction has been committed.</summary>
		public event EventHandler Committed;

		/// <summary>Invoked after the transaction has been rollbacked.</summary>
		public event EventHandler Rollbacked;

		/// <summary>Invoked after the transaction has closed and is disposed.</summary>
		public event EventHandler Disposed;
	}
}
