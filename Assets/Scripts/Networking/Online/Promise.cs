public class FutureBase {
    public enum Status {
        Idle, // reserved
        Running,
        Complete,
        Error
    }

    public Status status { get; protected set; }
    protected System.Action<string> onError;
    protected System.Action onFinally;
    protected string errorInfo;

    public FutureBase () {
        status = Status.Running;
    }
}

public class Future : FutureBase {
    protected System.Action onReturn;

    public Future OnReturn (System.Action onReturn) {
        if (status == Status.Complete) {
            onReturn();
        }
        this.onReturn += onReturn;
        return this;
    }

    public Future OnError (System.Action<string> onError) {
        if (status == Status.Error) {
            onError(errorInfo);
        }
        this.onError += onError;
        return this;
    }

    public Future OnFinally (System.Action onFinally) {
        if (status == Status.Error || status == Status.Complete) {
            onFinally();
        }
        this.onFinally += onFinally;
        return this;
    }
}

public class Promise : Future {
    public void Return () {
        if (status != Status.Running) {
            return;
        }
        status = Status.Complete;
        onReturn?.Invoke();
        onFinally?.Invoke();
    }

    public void Error (string errorInfo) {
        if (status == Status.Running) {
            status = Status.Error;
            this.errorInfo = errorInfo;
            onError?.Invoke(errorInfo);
            onFinally?.Invoke();
        }
    }
}

public class Future<T> : FutureBase {
    protected System.Action<T> onComplete;
    protected T data;

    public Future<T> OnReturn (System.Action<T> onComplete) {
        if (status == Status.Complete) {
            onComplete(data);
        }
        this.onComplete += onComplete;
        return this;
    }

    public Future<T> OnError (System.Action<string> onError) {
        if (status == Status.Error) {
            onError(errorInfo);
        }
        this.onError += onError;
        return this;
    }

    public Future<T> OnFinally (System.Action onFinally) {
        if (status == Status.Error || status == Status.Complete) {
            onFinally();
        }
        this.onFinally += onFinally;
        return this;
    }
}

public class Promise<T> : Future<T> {
    public void Return (T data) {
        if (status != Status.Running) {
            return;
        }
        this.data = data;
        status = Status.Complete;
        onComplete?.Invoke(data);
        onFinally?.Invoke();
    }

    public void Error (string errorInfo) {
        if (status == Status.Running) {
            status = Status.Error;
            this.errorInfo = errorInfo;
            onError?.Invoke(errorInfo);
            onFinally?.Invoke();
        }
    }
}
